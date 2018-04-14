using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using HackerNews.Models;

namespace HackerNews.Services
{
    public class HackerNewsService
    {
        // Define our cache timeout value
        const double CACHETIMEOUT = 60.0;

        /// <summary>
        /// Gets the list of story IDs from the HackerNews API
        /// </summary>
        /// <returns></returns>
        public static async Task<List<int>> GetBestStoriesIDs()
        {
            // Setup variables, cache, API URL
            ObjectCache cache = MemoryCache.Default;
            var respString = "";
            var stories = cache["storyIDs"] as List<int>;
            var apiURL = ConfigurationManager.AppSettings["HNBestStoriesURL"];

            // Check the cache if we already have our Story IDs
            if ( stories == null )
            {
                // Setup the expiration for the cache
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CACHETIMEOUT);

                // Now get the IDs from the API
                using (HttpClient httpClient = new HttpClient())
                {
                    // Await the response from the cient API
                    HttpResponseMessage response = await httpClient.GetAsync(apiURL);

                    if (response.IsSuccessStatusCode)
                    {
                        respString = await response.Content.ReadAsStringAsync();
                        stories = JsonConvert.DeserializeObject<List<int>>(respString);
                    }
                }
                // And now cache the data
                cache.Set("storyIDs", stories, policy);
            }
            return stories;
        }

        public static async Task<List<BestStoriesModel>> GetBestStories(List<int> storyIDs)
        {
            // No IDs, no results
            if(storyIDs.Count == 0) { return new List<BestStoriesModel>(); }

            // Setup our cache, API URLs and variables
            ObjectCache cache = MemoryCache.Default;
            var bestStores = cache["bestStories"] as List<BestStoriesModel>;
            var apiURL = ConfigurationManager.AppSettings["HNStoryDetailsURL"];

            // Check the cache if we already have the stories stored
            if (bestStores == null)
            {
                // Setup the expiration for the cache
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CACHETIMEOUT);

                // Setup our best stories list object
                bestStores = new List<BestStoriesModel>();

                // Loop through our story IDs and generate a list of best stories          
                using (HttpClient httpClient = new HttpClient())
                {
                    foreach (var storyID in storyIDs)
                    {
                        // Await the response from the cient API
                        HttpResponseMessage response = await httpClient.GetAsync(apiURL + storyID + ".json");

                        if (response.IsSuccessStatusCode)
                        {
                            var respData = await response.Content.ReadAsStringAsync();
                            var story = JsonConvert.DeserializeObject<HNStoryModel>(respData);
                            var hnstory = new BestStoriesModel();
                            hnstory.author = story.by;
                            hnstory.title = story.title;
                            bestStores.Add(hnstory);
                        }
                    }
                }
                // And now cache the data
                cache.Set("bestStories", bestStores, policy);
            }
            return bestStores;
        }
    }
}