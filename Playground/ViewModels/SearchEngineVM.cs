using Playground.Helpers;
using Playground.Interfaces;
using Playground.Models;
using Playground.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.ViewModels
{
    public class SearchEngineVM : ISearchEngineVM
    {
        #region Member variables
        // An observeable collection to store the results for displaying. When this list change it will be reflected in the view
        ICommand _doSearchCommand;
        ICommand _nextPageCommand;
        ICommand _previousPageCommand;

        public string DisplayMessage { get { return this.Helper.DisplayMessage; } }

        // The search count total
        public int SearchResultCount { get { return this.Helper.SearchResultCount; } }

        #endregion

        #region Properties
        public SearchEngineHelper Helper { get; }

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
        public ObservableCollection<SearchResult> Results => this.Helper.Results;
        #endregion

        #region Constructor

        /// <summary>
        /// The SearchEngineVM Constructor
        /// </summary>
        public SearchEngineVM() 
        {
            // Initalize our helper that handles the business logic 
            this.Helper = new SearchEngineHelper();

            // Initalise the commmand bindings
            _doSearchCommand = new DelegateCommand(this.Helper.DoSearch, this.Helper.CanDoSearch);
            _nextPageCommand = new DelegateCommand(this.Helper.GetPage, this.Helper.CanGetNextPage);
            _previousPageCommand = new DelegateCommand(this.Helper.GetPage, this.Helper.CanGetPreviousPage);
        }

        #endregion

        

             

        
    }
}
