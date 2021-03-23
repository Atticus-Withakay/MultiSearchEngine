using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using Playground.Interfaces;
using Playground.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Playground.ViewModels
{
    public class BingEngine : Notifier, ISearchEngine
    {
        private string _NextPageUrl;
        private string _PreviousPageUrl;

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

        public async Task GetNextPageAsync()
        {
            string content = await WebScraper.GetHTML(this.NextPageUrl, string.Empty);
            ParseResults(content);
        }
        public async Task GetPreviousPageAsync()
        {
            string content = await WebScraper.GetHTML(this.PreviousPageUrl, string.Empty);
            ParseResults(content);
        }

        public void ParseResults(string content)
        {
            /*
             * 
             * 
             <ol id="b_results">
                <li class="b_no">
                    <h1>There are no results for <strong>wonderwoman</strong></h1>
                    <ul>
                        <li><span>Check your spelling or try different keywords</span></li>
                    </ul>
                    <br>
                    <p>Ref A: 2843F521D842418F9A3BED20B3A2B8A5 Ref B: LTSEDGE1109 Ref C: 2021-03-21T12:45:27Z</p>
                </li>
             </ol>

             */
            try
            {
                // Clear old list
                this.Results.Clear();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);

                var document = htmlDoc.DocumentNode;


                //var resultsSection = htmlDoc.DocumentNode.SelectSingleNode("//ol[@id='b_results']");
                // Check for no results first!
                var noResults = document.QuerySelector("li.b_no");// resultsSection.SelectNodes("//[@class='b_no']");
                
                // the fact the "no results" class isn't present suggests there are some results.
                if (noResults == null)
                {

                    // Find the next page url

                    var next = HttpUtility.HtmlDecode(document.QuerySelector("a.sb_pagN")?.Attributes["href"].Value);
                    var prev = HttpUtility.HtmlDecode(document.QuerySelector("a.sb_pagP")?.Attributes["href"].Value);
                    
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
            
                    //var results = resultsSection.SelectNodes("//li[@class='b_algo']");
            
                    foreach (var result in results)
                    {
                        //TODO: this is too ridgid, need more checks to see if things exist before attempting things
                        var refNode = result.QuerySelector("a");// result.Element("h2").Element("a");
                        var url = refNode.Attributes["href"].Value;
                        var displayUrl = result.QuerySelector("cite").InnerText;// result.QuerySelector("cite").InnerText;
                        var text = refNode.InnerText;

                        //TODO: getting too much for the description. Need to be cases me thinks
                        var descriptionHtml = result.InnerText;// result.QuerySelector("p").InnerText;// HttpUtility.HtmlDecode(result.InnerText);
                        var description = HttpUtility.HtmlDecode(descriptionHtml);


                        Results.Add(new SearchResult(text, displayUrl, url, description, "Bing"));
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
