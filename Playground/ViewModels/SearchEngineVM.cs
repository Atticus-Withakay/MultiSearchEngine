using Playground.Interfaces;
using Playground.Models;
using Playground.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.ViewModels
{
    public class SearchEngineVM : ISearchEngineVM, INotifyPropertyChanged
    {
        // An observeable collection to store the results for displaying. When this list change it will be reflected in the view
        ObservableCollection<SearchResult> _results = new ObservableCollection<SearchResult>();
        ICommand _doSearchCommand;
        ICommand _nextPageCommand;
        ICommand _previousPageCommand;

        string _statusMessage;

        // List to contain search engines, allows for future expansion
        List<ISearchEngine> searchEngines;

        // The search count total
        int _searchResultCount;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public ICommand DoSearchCommand
        {
            get => _doSearchCommand;
            set => _doSearchCommand = value;
        }

        public ICommand NextPageCommand
        {
            get => _nextPageCommand;
            set => _nextPageCommand = value;
        }

        public ICommand PreviousPageCommand
        {
            get => _previousPageCommand;
            set => _previousPageCommand = value;
        }

        public ObservableCollection<SearchResult> Results {
            get => _results;
        }

        public string DisplayMessage
        {
            get { return _statusMessage; }
            set
            {
                this._statusMessage = value;
                OnNotifyPropertyChanged();
            }
        }

        public int SearchResultCount
        {
            get { return _searchResultCount; }
            set
            {
                this._searchResultCount = value;
                OnNotifyPropertyChanged();
            }
        }

        #endregion

        public SearchEngineVM() 
        {
            _doSearchCommand = new DelegateCommand(DoSearchAsync,
                                                   CanDoSearch);

            _nextPageCommand = new DelegateCommand(NextPageAsync, CanGetNextPage);
            _previousPageCommand = new DelegateCommand(PreviousPage, CanGetPreviousPage);

            _statusMessage = "Nothing to display, do a search";

            searchEngines = new List<ISearchEngine>();
            searchEngines.Add(new BingEngine());
            searchEngines.Add(new YahooEngine());
        }

        /// <summary>
        /// Checks if any of the search engines have a previous page link
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if there is a previous page url</returns>
        private bool CanGetPreviousPage(object obj)
        {
            return this.searchEngines.Any(engine => !string.IsNullOrEmpty(engine.PreviousPageUrl));
            //return string.IsNullOrEmpty(_bingEngine.PreviousPageUrl) || string.IsNullOrEmpty(_yahooEngine.PreviousPageUrl);
        }

        /// <summary>
        /// Checks if any of the search engines have a next page link
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if there is a next page url</returns>
        private bool CanGetNextPage(object obj)
        {
            return this.searchEngines.Any(engine => !string.IsNullOrEmpty(engine.NextPageUrl));

            //return string.IsNullOrEmpty(_bingEngine.NextPageUrl) || string.IsNullOrEmpty(_yahooEngine.NextPageUrl);
        }

        private async void PreviousPage(object obj)
        {

            foreach (var engine in this.searchEngines)
            {
                await engine.GetPreviousPageAsync();
            }
            InterleaveResults();
        }

        private async void NextPageAsync(object obj)
        {
            foreach (var engine in this.searchEngines)
            {
                await engine.GetNextPageAsync();
            }
            InterleaveResults();
        }

        private bool CanDoSearch(object obj)
        {
            //TODO: implement some form of validation to check search is okay to do.
            var valid = false;
            if (!string.IsNullOrEmpty(obj as string))
            {
                valid = true;
            }

            return valid;
        }

        private async void DoSearchAsync(object param)
        {
            // Update search message to show something is happening
            DisplayMessage = "Searching....";
            

            // Remove any white space from start/end
            var searchTerm = (param as string).Trim();

            var tasks = new List<Task>();
            // For each of our search engines do a search with our search term
            foreach (var engine in this.searchEngines)
            {
                tasks.Append(engine.DoSearchAsync(searchTerm));
            }

            await Task.WhenAll(tasks);
            InterleaveResults();
            // Builds our list of results but maintains search engine ranking

            
        }

        /// <summary>
        /// Function that takes each list of results from each search engine and merges them while maintaining search engine ranking.
        /// e.g. 1st bing, 1st yahoo, 2nd bing, 2nd yahoo if there were only 2 search engines
        /// 
        /// It will only add a result to the list if it exists as each search engine may return a different number of results
        /// </summary>
        private void InterleaveResults()
        {
            // Make sure our results list is clear ready for new results
            this._results.Clear();

            // Set our result counter
            this.SearchResultCount = this.searchEngines.Sum(engine => engine.Results.Count);

            // Find the maximum number of results from out list so we know how far to iterate
            var searchCount = this.searchEngines.Max(engine => engine.Results.Count);

            for (int i = 0; i < searchCount; i++)
            {
                foreach (var engine in this.searchEngines)
                {
                    var result = engine.Results.ElementAtOrDefault(i);
                    // Only add a result if it exists, different engines may return different number of results
                    if (result != null)
                    {
                        this.Results.Add(result);
                    }
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Finished search at: {DateTime.Now}");
            if (this._results.Count == 0)
            {
                stringBuilder.AppendLine("No search results");
            }
            DisplayMessage = stringBuilder.ToString();
        }     

        public void OnNotifyPropertyChanged([CallerMemberName] string name = "")//By using CallerMemberName no need to hard code property names, reduces errors! Obtains method/property name of caller to the method
        {
            // the ? means if PropertyChanged is null it wont invoke.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
