using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Models
{
    public class WebScraper
    {
        /// <summary>
        /// Function that will make a http reqest to a website
        /// </summary>
        /// <param name="baseUrl">The base search engine url e.g. www.bing.com/search </param>
        /// <param name="searchTerm">The search term</param>
        /// <returns></returns>
        public static async Task<string> GetHTML(string baseUrl, string searchTerm)
        {
            // Create a query param with our search term in it
            var queryString = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(searchTerm) )
            {
                queryString.Add("q", searchTerm);
            }

            // Turns our dictionary into q={queryString} and appends it to our base url. Fully encoded too
            var requestUri = QueryHelpers.AddQueryString(baseUrl, queryString);

            Console.WriteLine("Original URL: {0}", requestUri);

            var handler = new HttpClientHandler()
            {
                // Its possible that the original requst will get redirected, so need to allow that so we get the page we are wanting not just the first one that should be redirected.
                AllowAutoRedirect = true,
                // Set a limit and a cookie container, dont want to be caught in a redirect loop.
                MaxAutomaticRedirections = 100,
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            // Build our client
            var client = new HttpClient(handler);
            // Add User-agent header to make the request look kinda like its a human, this isnt perfect and should probably update periodically to "be human"
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
            // Avoids the need to open/close connection, it is what a real browser would have. Will help avoid anti scraping tech
            client.DefaultRequestHeaders.Connection.Add("Keep-Alive"); 


            // To further making it a "human" request add some more headers and params.
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            // Get the HTML
            var task = client.SendAsync(request).ContinueWith((taskwithMsg) =>
            {
                var response = taskwithMsg.Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // Prints the url and the status code to console for debugging, some better handling could be done here.
                    Console.WriteLine("RequestURI: {0}", response.RequestMessage.RequestUri);
                    Console.WriteLine("StatusCode: {0}", response.StatusCode);
                }

                var contentTask = response.Content.ReadAsStringAsync();
                contentTask.Wait();
                return contentTask.Result;
            });
            task.Wait();
            return task.Result;
        }
    }
}


//namespace Playground.Models
//{
//    public class WebScraper
//    {
//        /// <summary>
//        /// Function that will make a http reqest to a website
//        /// </summary>
//        /// <param name="baseUrl">The base search engine url e.g. www.bing.com/search </param>
//        /// <param name="searchTerm">The search term</param>
//        /// <returns></returns>
//        public static async Task<string> GetHTML(string baseUrl, string searchTerm)
//        {
//            // Create a query param with our search term in it
//            var queryString = new Dictionary<string, string>();

//            // Only build the search term query parameter if there is one
//            if (!string.IsNullOrEmpty(searchTerm))
//            {
//                queryString.Add("q", searchTerm);
//            }

//            // Turns our dictionary into q={queryString} and appends it to our base url. Fully encoded too
//            var requestUri = QueryHelpers.AddQueryString(baseUrl, queryString);

//            Console.WriteLine("Search URL: {0}", requestUri);

//            var handler = new HttpClientHandler()
//            {
//                // Its possible that the original requst will get redirected, so need to allow that so we get the page we are wanting not just the first one that should be redirected.
//                AllowAutoRedirect = true,
//                // Set a limit and a cookie container, dont want to be caught in a redirect loop.
//                MaxAutomaticRedirections = 100,
//                CookieContainer = new CookieContainer(),
//                UseCookies = true,
//                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
//            };

//            // Build our client
//            var client = new HttpClient(handler);
//            // Add User-agent header to make the request look kinda like its a human, this isnt perfect and should probably update periodically to "be human"
//            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
//            // Avoids the need to open/close connection, it is what a real browser would have. Will help avoid anti scraping tech
//            client.DefaultRequestHeaders.Connection.Add("Keep-Alive");


//            // To further making it a "human" request add some more headers and params.
//            var request = new HttpRequestMessage()
//            {
//                RequestUri = new Uri(requestUri),
//                Method = HttpMethod.Get,
//            };
//            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

//            // Get the HTML
//            var task = client.SendAsync(request).ContinueWith((taskwithMsg) =>
//            {
//                var response = taskwithMsg.Result;
//                if (response.StatusCode != HttpStatusCode.OK)
//                {
//                    // Prints the url and the status code to console for debugging, some better handling could be done here.
//                    Console.WriteLine("RequestURI: {0}", response.RequestMessage.RequestUri);
//                    Console.WriteLine("StatusCode: {0}", response.StatusCode);
//                }

//                var contentTask = response.Content.ReadAsStringAsync();
//                contentTask.Wait();
//                return contentTask.Result;
//            });

//            return Task.FromResult(task.Result);
//        }
//    }
//}
