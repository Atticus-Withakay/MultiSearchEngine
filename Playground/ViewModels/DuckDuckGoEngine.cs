using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using Playground.Interfaces;
using Playground.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Playground.ViewModels
{
    internal class DuckDuckGoEngine : ISearchEngine
    {
        public List<SearchResult> Results { get; set; }

        public DuckDuckGoEngine()
        {
            Results = new List<SearchResult>();
        }


        public async Task DoSearchAsync(string searchTerm)
        {
            this.Results.Clear();
            Console.WriteLine("Totally Doing some duck duck go serching!!!!");

            //All bing searches will have this as a base
            //var baseUrl = "https://duckduckgo.com/";
            var baseUrl = "https://uk.search.yahoo.com/search";
            string content = await WebScraper.GetHTML(searchTerm, baseUrl);

            ParseResults(content);



            var fakeResult = new SearchResult("Legit result", "www.legitResult.com", "This is a fake result, a placeholder if you will", "DuckDuckGo");
            Results.Add(fakeResult);
        }

        

        public void GetNextPage(string url)
        {
            throw new System.NotImplementedException();
        }

        public void ParseResults(string content)
        {
            Console.WriteLine("Parsing Ecosia");
        }
    }
}