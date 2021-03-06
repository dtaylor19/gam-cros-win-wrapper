﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChromebookGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Preferences.Init();

            MainWindow window = new MainWindow();
            window.Show();

            if(Preferences.PromptWhenUpdatesAreAvailable == true)
            {
                Updates.CheckForUpdates();
            }
            
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Preferences.Save();
        }
    }
}
