using System.Diagnostics;
using System.Windows;

namespace Playground.Views
{
    /// <summary>
    /// Interaction logic for SearchEngine.xaml
    /// </summary>
    public partial class SearchEngine : Window
    {
        public SearchEngine(ISearchEngineVM searchEngineVM)
        {
            InitializeComponent();
            this.DataContext = searchEngineVM;
        }

        /// <summary>
        /// Opens the hyperlinks in the defualt browser.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The event arguements</param>
        private void LinkNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var url = e.Uri.OriginalString;
            Process.Start(url);
            e.Handled = true;
        }
    }
}
