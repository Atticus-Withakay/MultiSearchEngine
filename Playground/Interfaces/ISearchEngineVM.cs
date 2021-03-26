using Playground.Helpers;
using Playground.Interfaces;
using Playground.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Playground.Interfaces
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
        SearchEngineHelper Helper { get; }

    }
}