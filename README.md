# MutliSearchEngine

The challenge was to create a search engine that would allow a user to enter a search term and then display results from 2 different search engines.

I have created a WPF C# applicate using MVVM structure that takes a search term from the user and then makes http requests and web scrapes to obtain search results that are then displayed in a list for the user to look at.

The search engines that I have implemented are Bing and Yahoo and the implementation i have done allows for more engines to be introduced the parser just needs to be defined and work with the current HTTP request setup.

## Tech/Libraries Used
- C#
- WPF, windows presentation foundation
- HtmlAgilityPack, a HTML selector/parser tool
- Fizzler.Systems.HtmlAgilityPack, adds extra functionality to html agility pack
- MVVM structure, using this allows for seperation of presentation, logic and storage and allows our app to be testable without going through the view. It also allows for databinding so the UI becomes reactive.

## How it works
A window is displayed with a text box, search button, list box and some next/previous buttons. The buttons are disabled until certain conditions are met:
- Search button only enabled when there is a non empty non white space search term present in the textbox
- Next/Previous btns only enable when a search engine object has a next/previous page url stored in it.

A display message is rendered in the middle of the window and only displays when there are no results to display, it updates depending on what is happening.
- Starts with "Nothing to display, do a search"
- While a search is in progress shows "Searching...."
- When complete it will be set to "Search complete at: {current time}" but will only display if there are no results! So ui gives feedback in the event of no results and you arent sitting there going "Has it broken?"

When the search button is clicked the term is passed to a background worker that kicks off each search engine search sequentially. Once the worker is complete the display list is updated and rendered. Each result has a title, url, description and source telling you which engine it came from. Clicking on the url will open the url in your default browser.

When a search is done if a Next page link is discoverd then the next button will be enabled and the user can cycle though pages. The same applies to the Previous page button.

## Assumptions
1. Each Search engine displays results slightly differently and may include ads or other suggested results. Therefore I have made a choice to only allow results that have a particular form:
- List elements with the class b_algo for Bing
- Div elements with the class algo for Yahoo

This means that suggested searches/images/ads wont be in the final list, If there was more time it should be possible to fetch these aswell and display appropriately but I didnt think they were proper search results so ignored them.

2. Search results are ranked so results matching your query are shown at the top and become less relevant as the pages go on. To maintain this ranking with each search engine the dispalyed list is interleaved so the 1st bing result is next to the 1st yahoo results not just all the bing results then all the yahoo results.
3. Each search engine may return different number of results per page therefore I interleave as far as I can then the rest of the list is populated with the remaining results from the larger results list.
4. To prevent bot detection I have Chosen not to get larger number of results e.g. using &count={number} for bing searches

## Challenges
- Not being able to use a search engine's API meant that I had to find another way of obtaining results. I decided to create a webscraper that would parse html returned my a HTTP request. This created a challenge in itself as most search engines implement anti webscraping techniques.
1. IP - If a website can see an ip address making a large number of requests in a short period of time there is a good chance that a bot is making the requests. I decided to emulate a browser by only making requests at the speed of the user. Results are only recovered a page at a time and parsed client side. Futher edits on this could be to intoduce a random delay placed between requests to make it look more "Human".
2. Captchas - If a website suspects you of being a bot it may show a capture to prove you are not a bot. I noticed this generally occurs when I was on a VPN, given the time constraint I wasnt able to create a auto capture filler so just disabled my vpn when using this app.
3. User-Agent - This is a header that alowes a website to identify how a user visits. It contains information about the OS and its version, CPU type, browser, and its version, browser language, a browser plug-in, etc. If a http request contains no headers it would identify itself as a script e.g. C# App a likely bot and would be blocked. So I populated it with my own header that matches a regular browser request e.g. "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36"
4. Redirection - Its possible to be redirected by a DNS to a different website so requests shouldnt be done by IP but by URL. This way our request gets sent to the correct location. Furthermore some sites will ask you to wait 5 seconds before being redirected to prove you're not a bot. This occured with ecosia and despite allowing redirection I was unable to get actual results. 

Taking the above into account and with the time allowed I was only able to obtain results from Bing & Yahoo. Google, DuckDuckGo, Ecosia have implemented anti webscraping techniques that I was unable to overcome in the time alloted.

- Seperating the UI thread from the logic thread but maintaining a reactive interface is always a challenge but is a key part of MVVM I achieved this by using Background workers to handle the business logic and using Observable collection and the INotifyPropertyChanged interface.

## Next steps
- It would be good to implement some unit tests. The html parser for each engine woudl be a good start. This way I would be able to tell if it breaks either from code modifications or if the original serach engine changes its html format. Given time restraits I wasnt able to do this but is something I would do.
- The Description parser could be improved, currently gets the inner text from a search result but the formating 
- Add a current page = x to the ui
- The background worker helps a lot with not blocking the UI but there is a bug where the previous button wouldnt enable until the window loses focus then gets refocused. My fix is just to reset the bindings but this is not a proper fix as it does that for all commmands not just the one that isnt updating. 
- Add a selection feature that would allow the user to make a "cart" of results and once review has been done display only the interesting results.
