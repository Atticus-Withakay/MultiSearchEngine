using Playground.ViewModels;
using Playground.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Playground
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Tells the application to start with a particular window.
        /// </summary>
        /// <param name="e">Startup event args</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Injects the view model into a the search engine view
            MainWindow = new SearchEngine(new SearchEngineVM());
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
