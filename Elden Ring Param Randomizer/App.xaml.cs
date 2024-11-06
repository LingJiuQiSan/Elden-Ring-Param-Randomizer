﻿using System.Windows;
using Elden_Ring_Param_Randomizer.Resources;
using SoulsFormats;

namespace Elden_Ring_Param_Randomizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is NoOodleFoundException)
            {
                MessageBox.Show($@"{Strings.NoOodleFoundException}
{e.Exception.Message}", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
            MessageBox.Show(e.Exception.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

    }

}
