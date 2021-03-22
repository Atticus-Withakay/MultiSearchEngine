using Playground.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Playground.Views
{
    public interface ISearchEngineVM
    {
        ICommand DoSearchCommand { get; }
        ICommand NextPageCommand { get; }
        ICommand PreviousPageCommand { get; }

        ObservableCollection<SearchResult> Results { get; }
    }
}