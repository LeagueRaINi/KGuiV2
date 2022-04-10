using KGuiV2.Interop;
using KGuiV2.Helpers;

using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System;
using System.Diagnostics;

namespace KGuiV2.ViewModels
{
    internal class AppViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The background update timer.
        /// </summary>
        DispatcherTimer _updateTimer;

        /// <summary>
        /// The tick at which the ramtest started.
        /// </summary>
        long _ramtestStartTick;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The ramtest cpu cache modes.
        /// </summary>
        public IEnumerable<Ramtest.CpuCacheMode> RamtestCpuCacheModes { get; } = Enum.GetValues(typeof(Ramtest.CpuCacheMode)).Cast<Ramtest.CpuCacheMode>();

        /// <summary>
        /// The ramtest rng modes.
        /// </summary>
        public IEnumerable<Ramtest.RngMode> RamtestRngModes { get; } = Enum.GetValues(typeof(Ramtest.RngMode)).Cast<Ramtest.RngMode>();

        /// <summary>
        /// Indicates if the ramtest is currently running.
        /// </summary>
        public bool RamtestIsRunning { get; set; }

        /// <summary>
        /// The amount of system memory in megabytes to test.
        /// </summary>
        public uint RamtestMegabytes { get; set; } = 1000;

        /// <summary>
        /// The amount of threads to use for the test.
        /// </summary>
        public uint RamtestThreads { get; set; } = (uint)(Environment.ProcessorCount / 2);

        /// <summary>
        /// The cpu cache mode to use for the ramtest.
        /// </summary>
        public Ramtest.CpuCacheMode RamtestCpuCacheMode { get; set; } = Ramtest.CpuCacheMode.Enabled;


        /// <summary>
        /// The rng mode to use for the ramtest.
        /// </summary>
        public Ramtest.RngMode RamtestRngMode { get; set; } = Ramtest.RngMode.Default;

        /// <summary>
        /// If the ramtest should also stress the cpu fpu.
        /// </summary>
        public bool RamtestStressFpu { get; set; }

        /// <summary>
        /// The speed at which the ramtest is running.
        /// </summary>
        public double RamtestSpeed => RamtestCoverage * (RamtestMegabytes / RamtestDuration.TotalSeconds);

        /// <summary>
        /// The current ramtest test duration.
        /// </summary>
        public TimeSpan RamtestDuration { get; set; }

        /// <summary>
        /// The current ramtest coverage.
        /// </summary>
        public double RamtestCoverage { get; set; }

        /// <summary>
        /// The current ramtest coverage in percent.
        /// </summary>
        public double RamtestCoveragePercent => 100 * RamtestCoverage;

        /// <summary>
        /// The next full ramtest coverage in percent.
        /// </summary>
        public double RamtestNextFullCoveragePercent => 100 * Math.Floor(RamtestCoverage + 1.0);

        /// <summary>
        /// The approximate time it will take to reach the next full coverage percentage.
        /// </summary>
        public TimeSpan RamtestNextFullCoverageIn => TimeSpan.FromSeconds((int)(0.01 * (RamtestNextFullCoveragePercent - RamtestCoveragePercent) * RamtestMegabytes / RamtestSpeed));

        /// <summary>
        /// The approximate time it will take for the test to finish with the given <see cref="RamtestTaskScope"/>.
        /// </summary>
        public TimeSpan RamtestFinishedIn => TimeSpan.FromSeconds((int)(0.01 * (RamtestTaskScope - RamtestCoveragePercent) * RamtestMegabytes / RamtestSpeed));

        /// <summary>
        /// Indicates if the test should be stopped if <see cref="RamtestTaskScope"/> is reached.
        /// </summary>
        public bool RamtestStopOnTaskScope { get; set; } = true;

        /// <summary>
        /// The task scope at which to stop testing.
        /// </summary>
        public double RamtestTaskScope { get; set; } = 10000;

        /// <summary>
        /// Indicates if the test should be stopped if an error is detected.
        /// </summary>
        public bool RamtestStopOnError { get; set; } = true;

        /// <summary>
        /// Indicates if <see cref="Console.Beep"/> should be used if the test encounters an error and <see cref="RamtestStopOnError"/> is enabled.
        /// </summary>
        public bool RamtestBeepOnError { get; set; } = true;

        /// <summary>
        /// The amount of ramtest errors.
        /// </summary>
        public uint RamtestErrorCount { get; set; }

        /// <summary>
        /// The total amount of system memory.
        /// </summary>
        public double SystemMemoryTotal { get; set; }

        /// <summary>
        /// The amount of currently unused system memory.
        /// </summary>
        public double SystemMemoryFree { get; set; }

        /// <summary>
        /// The percentage of currently unused system memory.
        /// </summary>
        public double SystemMemoryFreePercentage => 100 * (SystemMemoryFree / SystemMemoryTotal);

        /// <summary>
        /// The total number of cpu threads.
        /// </summary>
        public uint SystemCpuThreads = (uint)(Environment.ProcessorCount);

        /// <summary>
        /// Command for starting the ramtest.
        /// </summary>
        public ICommand RamtestStartCommand { get; }

        /// <summary>
        /// Command for stopping the ramtest.
        /// </summary>
        public ICommand RamtestStopCommand { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AppViewModel"/>.
        /// </summary>
        public AppViewModel()
        {
            _updateTimer = new DispatcherTimer(DispatcherPriority.Background);
            _updateTimer.Interval = TimeSpan.FromMilliseconds(150);
            _updateTimer.Tick += UpdaterOnTick;
            _updateTimer.Start();

            RamtestStartCommand = new RelayCommand(CanStartRamtest, StartRamtest);
            RamtestStopCommand = new RelayCommand(CanStopRamtest, StopRamtest);
        }

        /// <summary>
        /// Checks if the ramtest can be stopped.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool CanStopRamtest(object? param)
            => RamtestIsRunning;

        /// <summary>
        /// Checks if the ramtest can be started.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool CanStartRamtest(object? param)
            => !RamtestIsRunning
            && RamtestMegabytes >= 50
            && RamtestMegabytes <= SystemMemoryFree
            && RamtestThreads > 0
            && RamtestThreads <= Environment.ProcessorCount;

        /// <summary>
        /// Stops the ramtest.
        /// </summary>
        /// <param name="param"></param>
        void StopRamtest(object? param)
            => RamtestIsRunning = !Ramtest.StopTest();

        /// <summary>
        /// Configures and starts the ramtest.
        /// </summary>
        /// <param name="param"></param>
        void StartRamtest(object? param)
        {
            Ramtest.SetStressFpu(RamtestStressFpu);
            Ramtest.SetCpuCache(RamtestCpuCacheMode);
            Ramtest.SetRng(RamtestRngMode);

            var started = RamtestIsRunning = Ramtest.StartTest(RamtestMegabytes, RamtestThreads);
            if (started)
                _ramtestStartTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// The background updater method used for updating variales.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpdaterOnTick(object? sender, EventArgs e)
        {
            var memoryStatusEx = new Kernel32.MemoryStatusEx();

            if (Kernel32.GlobalMemoryStatusEx(ref memoryStatusEx))
            {
                SystemMemoryTotal = Convert.ToDouble(memoryStatusEx.TotalPhys / 1024 / 1024);
                SystemMemoryFree = Convert.ToDouble(memoryStatusEx.AvailPhys / 1024 / 1024);
            }

            if (RamtestIsRunning)
            {
                RamtestErrorCount = Ramtest.GetErrorCount();
                RamtestCoverage = Ramtest.GetCoverage();
                RamtestDuration = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _ramtestStartTick);

                if ((RamtestStopOnError && RamtestErrorCount > 0) || (RamtestStopOnTaskScope && RamtestTaskScope <= RamtestCoveragePercent))
                {
                    // TODO!: seems wank
                    if (RamtestStopCommand.CanExecute(null))
                        RamtestStopCommand.Execute(null);

                    if (RamtestBeepOnError && RamtestErrorCount > 0)
                        Console.Beep(1000, 300);
                }
            }
        }
    }
}
