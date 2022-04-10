using KGuiV2.ViewModels;

using System.Threading;
using System.Windows;
using System;

namespace KGuiV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The app mutex is for identifying if an instance of our app is already running.
        /// </summary>
        Mutex _instanceMutex;

        /// <summary>
        /// Gets called when our application starts up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_OnStartup(object sender, StartupEventArgs e)
        {
            _instanceMutex = new Mutex(true, "Local\\KGui", out var isNewMutex);

            if (!isNewMutex)
            {
                // TODO!: try to find the process and call SetForegroundWindow on it
                Environment.Exit(0);
            }

            GC.KeepAlive(_instanceMutex);

            this.MainWindow = new MainWindow();
            this.MainWindow.DataContext = new AppViewModel();
            this.MainWindow.Show();
        }
    }
}
