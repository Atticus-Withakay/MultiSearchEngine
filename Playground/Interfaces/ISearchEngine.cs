using HtmlAgilityPack;
using Playground.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playground.Interfaces
{
    public interface ISearchEngine
    {
        string NextPageUrl { get; set; }
        string PreviousPageUrl { get; set; }
        string BaseUrl { get; set; }


        /// <summary>
        /// List to store results
        /// </summary>
        List<SearchResult> Results { get; set; }
        /// <summary>
        /// Function that takes a search term and then 
        /// </summary>
        /// <param name="searchTerm"></param>
        Task DoSearchAsync(string searchTerm);
        /// <summary>
        /// Fetches
        /// </summary>
        /// <param name="url"></param>
        Task GetNextPageAsync();
        Task GetPreviousPageAsync();


        void ParseResults(string content);
    }
}
