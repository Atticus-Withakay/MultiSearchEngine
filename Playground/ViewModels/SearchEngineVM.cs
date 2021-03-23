using Playground.Interfaces;
using Playground.Models;
using Playground.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Playground.ViewModels
{
    public class SearchEngineVM : Notifier, ISearchEngineVM
    {
        #region Member variables
        // An observeable collection to store the results for displaying. When this list change it will be reflected in the view
        ObservableCollection<SearchResult> _results = new ObservableCollection<SearchResult>();
        ICommand _doSearchCommand;
        ICommand _nextPageCommand;
        ICommand _previousPageCommand;
        ObservableCollection<ISearchEngine> _searchEngines;

        string _DisplayMessage;

        // The search count total
        int _searchResultCount;

        #endregion

        #region Properties
        // List to contain search engines, allows for future expansion
        public ObservableCollection<ISearchEngine> SearchEngines
        {
            get { return _searchEngines; }
            set
            {
                this._searchEngines = value;
            }
        }

        /// <summary>
        /// Gets or sets the DoSearch command
        /// </summary>
        public ICommand DoSearchCommand
        {
            get => _doSearchCommand;
            set => _doSearchCommand = value;
        }

        /// <summary>
        /// Gets or sets the next page command
        /// </summary>
        public ICommand NextPageCommand
        {
            get => _nextPageCommand;
            set => _nextPageCommand = value;
        }

        /// <summary>
        /// Gets or sets the previous page command
        /// </summary>
        public ICommand PreviousPageCommand
        {
            get => _previousPageCommand;
            set => _previousPageCommand = value;
        }

        /// <summary>
        /// Gets the search results list
        /// </summary>
        public ObservableCollection<SearchResult> Results {
            get => _results;
        }

        /// <summary>
        /// Gets or sets the status message and updates the ui
        /// </summary>
        public string DisplayMessage
        {
            get { return _DisplayMessage; }
            set
            {
                this._DisplayMessage = value;
                OnNotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the search result count of the currently displayed results
        /// </summary>
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

        #region Constructor
        /// <summary>
        /// The SearchEngineVM Constructor
        /// </summary>
        public SearchEngineVM() 
        {
            // Initalise the commmands
            _doSearchCommand = new DelegateCommand(DoSearch, CanDoSearch);
            _nextPageCommand = new DelegateCommand(GetPage, CanGetNextPage);
            _previousPageCommand = new DelegateCommand(GetPage, CanGetPreviousPage);
            // Set the starting display image
            _DisplayMessage = "Nothing to display, do a search";

            // Populate our search engine list
            SearchEngines = new ObservableCollection<ISearchEngine>();
            SearchEngines.Add(new BingEngine());
            //SearchEngines.Add(new YahooEngine());
        }
        #endregion

        #region Action & Predicates
        /// <summary>
        /// Checks if any of the search engines have a previous page link
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if there is a previous page url, otherwise disables the previous button</returns>
        private bool CanGetPreviousPage(object obj)
        {
            return this.SearchEngines.Any(engine => !string.IsNullOrEmpty(engine.PreviousPageUrl));
        }

        /// <summary>
        /// Checks if any of the search engines have a next page link
        /// </summary>
        /// <param name="obj">Required by pedicate but serves no use for this particular implementation</param>
        /// <returns>True if there is a next page url, otherwise disables the next button</returns>
        private bool CanGetNextPage(object obj)
        {
            return this.SearchEngines.Any(engine => !string.IsNullOrEmpty(engine.NextPageUrl));
        }


        /// <summary>
        /// Function that will build a background worker to fetch a page of results based on the page type e.g. Next or previous
        /// </summary>
        /// <param name="obj">The param encapsulated by the action, should store a string</param>
        public void GetPage(object obj)
        {
            var pageType = obj as string;
            // Build a background worker 
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += GetPageWorkerAsync; // Main task is the search   
            worker.RunWorkerCompleted += worker_RunInterleave; // Once main task, this function is called 
            worker.RunWorkerAsync(argument: pageType); // Start the worker with our search term passed in as an argument.
        }

        /// <summary>
        /// Predicate that checks that its possible to do a search
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanDoSearch(object obj)
        {
            //TODO: implement some form of validation to check search is okay to do.
            var valid = false;
            var searchTerm = obj as string;

            //checked if text and not just white space
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                valid = true;
            }

            return valid;
        }

        /// <summary>
        /// Action that will build a background worker to do the search
        /// </summary>
        /// <param name="param">The action data, should contain the search term</param>
        public void DoSearch(object param)
        {
            // Update search message to show something is happening
            DisplayMessage = "Searching....";

            // As we are starting a search make sure our results are empty ready for our next batch
            this.Results.Clear();

            // Remove any white space from start/end
            var searchTerm = (param as string).Trim();

            // Build a background worker 
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoSearchAsync; // Adds a handler for our main task the search   
            worker.RunWorkerCompleted += worker_RunInterleave; // Once main task is completed this function will be called 
            worker.RunWorkerAsync(argument: searchTerm); // Start the worker with our search term passed in as an argument.
        }

        #endregion

        #region Background workers
        /// <summary>
        /// Asyc function that will fetch either the next or previous page of results
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">Event arguement data</param>
        private async void GetPageWorkerAsync(object sender, DoWorkEventArgs e)
        {           
            var type = e.Argument as string;
            // Foreach search engine get a page
            foreach (var engine in this.SearchEngines)
            {
                // Check the arg type to know which direction we are going
                if (type == "Next")
                {
                    await engine.GetNextPageAsync();
                }
                else
                {
                    await engine.GetPreviousPageAsync();
                }
            }
        }
        
        /// <summary>
        /// The search worker done event handler
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">Event arguement data</param>
        private void worker_RunInterleave(object sender, RunWorkerCompletedEventArgs e)
        {
            InterleaveResults();
            // There is a bug where my previous button can execute function is running before the previous url is set. Probably due to work thread ui thread issues. This is a quick fix to demo the app, with more time would be fixed.
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// The search worker event handler
        /// </summary>
        /// <param name="sender">The object that called the event</param>
        /// <param name="e">Event arguement data</param>
        private async void worker_DoSearchAsync(object sender, DoWorkEventArgs e)
        {
            var searchTerm = e.Argument as string;

            foreach (var engine in this.SearchEngines)
            {
                await engine.DoSearchAsync(searchTerm);
            }
        }
        #endregion

        /// <summary>
        /// Function that takes each list of results from each search engine and merges them while maintaining search engine ranking.
        /// e.g. 1st bing, 1st yahoo, 2nd bing, 2nd yahoo if there were only 2 search engines
        /// 
        /// It will only add a result to the list if it exists as each search engine may return a different number of results
        /// </summary>
        private void InterleaveResults()
        {
            // Make sure our results list is clear ready for new results
            this.Results.Clear();

            //List<SearchResult> res = new List<SearchResult>();

            // Set our result counter
            this.SearchResultCount = this.SearchEngines.Sum(engine => engine.Results.Count);

            // Find the maximum number of results from out list so we know how far to iterate
            var searchCount = this.SearchEngines.Max(engine => engine.Results.Count);

            for (int i = 0; i < searchCount; i++)
            {
                foreach (var engine in this.SearchEngines)
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
            if (this.Results.Count == 0)
            {
                    stringBuilder.AppendLine("No search results");
            }

            // The display message is only visible when Results.Count is 0 but good to have some details saved each time
            DisplayMessage = stringBuilder.ToString();
        }     

        
    }
}
