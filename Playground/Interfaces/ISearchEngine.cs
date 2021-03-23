using Playground.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Interfaces
{
    public interface ISearchEngine
    {
        /// <summary>
        /// List to store results
        /// </summary>
        List<SearchResult> Results { get; set; }
        string NextPageUrl { get; set; }
        string PreviousPageUrl { get; set; }
        string BaseUrl { get; set; }


        /// <summary>
        /// Function that takes a search term and then 
        /// </summary>
        /// <param name="searchTerm"></param>
        Task DoSearchAsync(string searchTerm);

        /// <summary>
        /// Gets the next page of results
        /// </summary>
        /// <param name="url"></param>
        Task GetNextPageAsync();

        /// <summary>
        /// Gets the previous page of results
        /// </summary>
        /// <returns></returns>
        Task GetPreviousPageAsync();

        /// <summary>
        /// Parses HTML content to extract results
        /// </summary>
        /// <param name="content"></param>
        void ParseResults(string content);
    }
}
