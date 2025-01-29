using System.Net;
using Newtonsoft.Json;

using Assignment_A2_04.Models;
using System.Collections.Concurrent;

namespace Assignment_A2_04.Services;
public class NewsService
{
    readonly string _subscriptionKey = "256970bad92b4d5398613d17fcba4a7f";
    readonly string _endpoint = "https://api.bing.microsoft.com/v7.0/news";
    readonly HttpClient _httpClient = new HttpClient();

    //Event that is triggerd when news is available
    public event EventHandler<string> NewsAvailable;
    protected virtual void OnNewsAvailable(string message)
    {
        NewsAvailable?.Invoke(this, message);
    }
    public NewsService()
    {
        _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
    }
    public async Task<NewsResponse> GetNewsAsync(NewsCategory category)
    {
        //Cache key based on category and DateTime
        var cacheKey = new NewsCacheKey(category, DateTime.Now);

        //Checks if news is found i cache and not older then 1 minute
        if (cacheKey.CacheExist && (DateTime.Now - File.GetLastWriteTime(cacheKey.FileName)).TotalMinutes<=1)
        {
            //Triggers the event and returns news from XML cache
            OnNewsAvailable($"News for category {category} is available in XML cache");
            var NewsAvailableCached = NewsResponse.Deserialize(cacheKey.FileName);

            return NewsAvailableCached;
        }
        try
        {
            //Make the http request and ensure success
            string uri = $"{_endpoint}?mkt=en-us&category={Uri.EscapeDataString(category.ToString())}";
            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            //To ensure not too many requests per second for BingNewsApi free plan
            await Task.Delay(2000);

            //Convert Json to NewsResponse
            string content = await response.Content.ReadAsStringAsync();
            var newsResponse = JsonConvert.DeserializeObject<NewsResponse>(content);
            newsResponse.Category = category;

            //Serialize data to XML
            NewsResponse.Serialize(newsResponse, cacheKey.FileName);

            //Trigger the event
            if (newsResponse != null)
            {
                OnNewsAvailable($"News for category {category} is available");
            }
            else
            {
                OnNewsAvailable($"News for category {category} is not available");
            }

            return newsResponse;
        }
        //This catch will catch errors related to HTTP requests
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"A HTTP error has occurred: {httpEx.Message} ");
            throw;
        }
        //This catch will catch errors related to JSON deserialization
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"A JAON Deserialization error has occurred: {jsonEx.Message}");
            throw;
        }
        //This catch will catch any other exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occurred: {ex.Message}");
            throw;
        }
    }
}