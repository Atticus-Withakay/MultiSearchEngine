using HtmlAgilityPack;
using Playground.Interfaces;
using Playground.Models;
using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using System.Web;
using System.Linq;
using System.Threading.Tasks;

namespace Playground.Models
{
    internal class YahooEngine : ISearchEngine
    {
        public List<SearchResult> Results { get; set; }
        public string NextPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }
        public string BaseUrl { get; set; }

        private const string Source = "Yahoo";

        public YahooEngine()
        {
            Results = new List<SearchResult>();
            this.BaseUrl = "https://uk.search.yahoo.com";
        }


        /// <summary>
        /// Does a bing search
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns></returns>
        public async Task DoSearchAsync(string searchTerm)
        {
            this.Results.Clear();
            Console.WriteLine("Totally Doing some searching!!!!");

            //All yahoo searches will have this as a base
            var searchUrl = this.BaseUrl + "/search";// "https://uk.search.yahoo.com/search";
            string content = await WebScraper.GetHTML(searchUrl, searchTerm);

            ParseResults(content);
        }

        /// <summary>
        ///  Gets the next Bing page
        /// </summary>
        /// <returns></returns>
        public async Task GetNextPageAsync()
        {
            string content = await WebScraper.GetHTML(this.NextPageUrl, string.Empty);
            ParseResults(content);
        }

        /// <summary>
        /// Gets the previouys Bing page
        /// </summary>
        /// <returns></returns>
        public async Task GetPreviousPageAsync()
        {
            string content = await WebScraper.GetHTML(this.PreviousPageUrl, string.Empty);
            ParseResults(content);
        }

        /// <summary>
        /// Parses a Bing search page
        /// </summary>
        /// <param name="content">The html</param>
        public void ParseResults(string content)
        {
            // Since we are parsing html lets make our results list blank.
            this.Results.Clear();

            // Turn content which is a string into a html doc for easy parsing
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            var document = htmlDoc.DocumentNode;
            var results = document.QuerySelectorAll("div.algo");

            // Only process if results exist
            if (results != null && results.ToList().Count > 0)
            {
                try
                {
                    // Find the next page url, make sure to decode special characters
                    this.NextPageUrl = HttpUtility.HtmlDecode(document.QuerySelector("a.next")?.Attributes["href"].Value);  
                    this.PreviousPageUrl = HttpUtility.HtmlDecode(document.QuerySelector("a.prev")?.Attributes["href"].Value); 

                    foreach (var result in results)
                    {
                        var anchor = result.QuerySelector("a");

                        // Yahoo href link is very long and hard to read
                        var url = anchor.Attributes["href"].Value;
                        // Get the friendly Display version that actually says where it is going
                        var displayUrl = result.QuerySelector("span.fz-ms")?.InnerText;

                        // Gets header text
                        var text = result.QuerySelector("h3")?.InnerText;
                        
                        // Gets the general description/summary of the link
                        var description = result.QuerySelector("p.fz-ms")?.InnerText;

                        // Decodes html encoded special characters
                        description = HttpUtility.HtmlDecode(description);

                        // In case no description on a result
                        if (string.IsNullOrEmpty(description))
                        {
                            description = "Not applicable";
                        }

                        Results.Add(new SearchResult(text, displayUrl, url, description, Source));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Its gone wrong");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}