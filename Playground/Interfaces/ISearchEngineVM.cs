using Playground.Interfaces;
using Playground.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Playground.Views
{
    /// <summary>
    /// Interface to define a search engine view model
    /// </summary>
    public interface ISearchEngineVM
    {
        ObservableCollection<SearchResult> Results { get; }
        ICommand DoSearchCommand { get; }
        ICommand NextPageCommand { get; }
        ICommand PreviousPageCommand { get; }

        ObservableCollection<ISearchEngine> SearchEngines { get; set; }

        /// <summary>
        /// Action method to do a search
        /// </summary>
        /// <param name="param"></param>
        void DoSearch(object param);
        /// <summary>
        /// Action method to get a page
        /// </summary>
        /// <param name="param"></param>
        void GetPage(object param);

    }
}