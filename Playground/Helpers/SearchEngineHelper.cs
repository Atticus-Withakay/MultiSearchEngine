using Playground.Interfaces;
using Playground.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Playground.Helpers
{
    public class SearchEngineHelper : Notifier
    {
        private string _DisplayMessage;
        private int _SearchResultCount;
        private ObservableCollection<SearchResult> _Results;

        // List to contain search engines, allows for future expansion
        List<ISearchEngine> Engines { get; set; }

        public ObservableCollection<SearchResult> Results {
            get { return this._Results; }
            set 
            {
                this._Results = value;
            } 
        }

        /// <summary>
        /// Gets or sets the search result count of the currently displayed results
        /// </summary>
        public int SearchResultCount
        {
            get { return _SearchResultCount; } 
            set 
            {
                this._SearchResultCount = value;
                OnNotifyPropertyChanged();
            }  
        }


        /// <summary>
        /// Gets or sets the status message and updates the ui
        /// </summary>
        public string DisplayMessage { 
            get { return this._DisplayMessage; } 
            private set 
            { 
                this._DisplayMessage = value;
                OnNotifyPropertyChanged();
            }
        }

        public SearchEngineHelper()
        {
            Results = new ObservableCollection<SearchResult>();
            Engines = new List<ISearchEngine>
            {
                new BingEngine(),
                new YahooEngine()
            };
            this.SearchResultCount = 1000;
            // Set the starting display image
            DisplayMessage = "Nothing to display, do a search";
        }

        #region Background workers
        /// <summary>
        /// Asyc function that will fetch either the next or previous page of results
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">Event arguement data</param>
        private void Worker_GetPages(object sender, DoWorkEventArgs e)
        {
            var type = e.Argument as string;
            var taskList = new List<Task>();
            // Foreach search engine get a page
            foreach (var engine in this.Engines)
            {
                var task = new Task(() =>
                {
                    // Check the arg type to know which direction we are going
                    if (type == "Next")
                    {
                        engine.GetNextPageAsync();
                    }
                    else
                    {
                        engine.GetPreviousPageAsync();
                    }
                });
                taskList.Add(task);
                task.Start();
            }
            Task.WaitAll(taskList.ToArray());
        }

        /// <summary>
        /// The search worker done event handler
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">Event arguement data</param>
        private void worker_RunInterleave(object sender, RunWorkerCompletedEventArgs e)
        {
            InterleaveResults();
            foreach (var engine in this.Engines)
            {
                Console.WriteLine("The next page is: {0}", engine.NextPageUrl);
                Console.WriteLine("The previous page is: {0}", engine.PreviousPageUrl);
            }

            //// There is a bug where my previous button can execute function is running before the previous url is set. Probably due to work thread ui thread issues. This is a quick fix to demo the app, with more time would be fixed.
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// The search worker event handler
        /// </summary>
        /// <param name="sender">The object that called the event</param>
        /// <param name="e">Event arguement data</param>
        private void Worker_DoSearch(object sender, DoWorkEventArgs e)
        {
            var searchTerm = e.Argument as string;
            var taskList = new List<Task>();
            foreach (var engine in this.Engines)
            {
                var task = new Task(() =>
                {
                    engine.DoSearchAsync(searchTerm);
                });
                taskList.Add(task);
                task.Start();
            }
            Task.WaitAll(taskList.ToArray());
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
            this.Results.Clear();
            // Set our result counter
            this.SearchResultCount = this.Engines.Sum(engine => engine.Results.Count);

            // Find the maximum number of results from out list so we know how far to iterate
            var longestResults = this.Engines.Max(engine => engine.Results.Count);

            for (int i = 0; i < longestResults; i++)
            {
                foreach (var engine in this.Engines)
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
            Console.WriteLine(this.DisplayMessage);
        }


        #region Action & Predicates
        /// <summary>
        /// Checks if any of the search engines have a previous page link
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if there is a previous page url, otherwise disables the previous button</returns>
        public bool CanGetPreviousPage(object obj)
        {
            var temp = this.Engines.Any(engine => !string.IsNullOrEmpty(engine.PreviousPageUrl));
            return temp;
        }

        /// <summary>
        /// Checks if any of the search engines have a next page link
        /// </summary>
        /// <param name="obj">Required by pedicate but serves no use for this particular implementation</param>
        /// <returns>True if there is a next page url, otherwise disables the next button</returns>
        public bool CanGetNextPage(object obj)
        {
            return this.Engines.Any(engine => !string.IsNullOrEmpty(engine.NextPageUrl));
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
            worker.DoWork += Worker_GetPages; // Main task is the search   
            worker.RunWorkerCompleted += worker_RunInterleave; // Once main task, this function is called 
            worker.RunWorkerAsync(argument: pageType); // Start the worker with our search term passed in as an argument.
        }

        /// <summary>
        /// Predicate that checks that its possible to do a search
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanDoSearch(object obj)
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
            var searchTerm = (param as string).Trim();

            // Update search message to show something is happening
            DisplayMessage = "Searching....";

            //// As we are starting a search make sure our results are empty ready for our next batch
            //this.Results.Clear();

            // Build a background worker 
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoSearch; // Adds a handler for our main task the search   
            worker.RunWorkerCompleted += worker_RunInterleave; // Once main task is completed this function will be called 
            //worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync(argument: searchTerm); // Start the worker with our search term passed in as an argument.

        }

        #endregion
    }
}
