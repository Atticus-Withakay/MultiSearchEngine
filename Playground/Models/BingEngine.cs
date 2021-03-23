using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Playground.Interfaces;
using Playground.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Playground.ViewModels
{
    public class BingEngine : Notifier, ISearchEngine
    {
        private string _NextPageUrl;
        private string _PreviousPageUrl;

        private const string Source = "Bing";

        #region Properties
        public List<SearchResult> Results { get; set; }
        public string NextPageUrl { 
            get { return _NextPageUrl;  } 
            set 
            {
                this._NextPageUrl = value;
                OnNotifyPropertyChanged();
            } 
        }
        public string PreviousPageUrl
        {
            get { return _PreviousPageUrl; }
            set
            {
                this._PreviousPageUrl = value;
                OnNotifyPropertyChanged();
            }
        }

        public string BaseUrl { get; set; }
        #endregion

        public BingEngine()
        {
            this.BaseUrl = "https://www.bing.com";
            this.Results = new List<SearchResult>();
        }

        /// <summary>
        /// Does a bing search, parses the returned html and adds it to Results
        /// </summary>
        /// <param name="searchTerm">The search query</param>
        /// <returns></returns>
        public async Task DoSearchAsync(string searchTerm)
        {
            //All bing searches will have this as a base
            var searchUrl = this.BaseUrl + "/search"; 

            // Make out request
            string content = await WebScraper.GetHTML(searchUrl, searchTerm);
      
            ParseResults(content);
        }

        /// <summary>
        /// Gets the next page content
        /// </summary>
        /// <returns></returns>
        public async Task GetNextPageAsync()
        {
            string content = await WebScraper.GetHTML(this.NextPageUrl, string.Empty);
            ParseResults(content);
        }

        /// <summary>
        /// Gets the previous page content
        /// </summary>
        /// <returns></returns>
        public async Task GetPreviousPageAsync()
        {
            string content = await WebScraper.GetHTML(this.PreviousPageUrl, string.Empty);
            ParseResults(content);
        }

        /// <summary>
        /// Parses the html content
        /// </summary>
        /// <param name="content">The html</param>
        public void ParseResults(string content)
        {
            try
            {
                // Clear old list
                this.Results.Clear();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);
                var document = htmlDoc.DocumentNode;

                // Check for no results first!
                var noResults = document.QuerySelector("li.b_no");// resultsSection.SelectNodes("//[@class='b_no']");
                
                // the fact the "no results" class isn't present suggests there are some results.
                if (noResults == null)
                {

                    // Find the next page url
                    var next = HttpUtility.HtmlDecode(document.QuerySelector("a.sb_pagN")?.Attributes["href"].Value);
                    // Find the previous page url
                    var prev = HttpUtility.HtmlDecode(document.QuerySelector("a.sb_pagP")?.Attributes["href"].Value);
                    
                    // Handle null references
                    if (string.IsNullOrEmpty(next))
                    {
                        this.NextPageUrl = string.Empty;
                    }
                    else
                    {
                        this.NextPageUrl = this.BaseUrl + next;
                    }
                    if (string.IsNullOrEmpty(prev))
                    {
                        this.PreviousPageUrl = string.Empty;
                    }
                    else
                    {
                        this.PreviousPageUrl = this.BaseUrl + prev;
                    }

                    // Gets the list elements with the .b_algo class. As far as i 
                    var results = document.QuerySelectorAll("li.b_algo");
            
                    foreach (var result in results)
                    {
                        var refNode = result.QuerySelector("a");
                        var url = refNode.Attributes["href"].Value;
                        var displayUrl = result.QuerySelector("cite").InnerText;
                        var text = refNode.InnerText;

                        //TODO: getting too much for the description. Parsing can be improved 
                        var descriptionHtml = result.InnerText;
                        var description = HttpUtility.HtmlDecode(descriptionHtml);

                        Results.Add(new SearchResult(text, displayUrl, url, description, Source));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Its gone wrong");
                Console.WriteLine(ex.Message);
                // Would be good to have a notification window appear with the exact error.
            }
        }
    }
}
