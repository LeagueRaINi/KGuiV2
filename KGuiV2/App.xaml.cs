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
        /// Gets called when our application starts up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_OnStartup(object sender, StartupEventArgs e)
        {
            using (var mutex = new Mutex(true, "Local\\KGui"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    // TODO!
                    Environment.Exit(0);
                }

                MainWindow = new MainWindow();
                MainWindow.DataContext = new AppViewModel();
                MainWindow.ShowDialog();
            }
        }
    }
}
