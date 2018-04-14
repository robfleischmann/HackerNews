using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using HackerNews.Models;

namespace HackerNews.Services
{
    public class HackerNewsService
    {
        /// <summary>
        /// Gets the list of story IDs from the HackerNews API
        /// </summary>
        /// <returns></returns>
        public static async Task<List<int>> GetBestStoriesIDs()
        {
            var stories = new List<int>();
            var respString = "";
            // Get the API URL for retrieving best story IDs
            var apiURL = ConfigurationManager.AppSettings["HNBestStoriesURL"];

            using (HttpClient httpClient = new HttpClient())
            {
                // Await the response from the cient API
                HttpResponseMessage response = await httpClient.GetAsync(apiURL);

                if(response.IsSuccessStatusCode)
                {
                    respString = await response.Content.ReadAsStringAsync();
                    stories = JsonConvert.DeserializeObject<List<int>>(respString);
                }
            }
            return stories;
        }

        public static async Task<List<BestStoriesModel>> GetBestStories(List<int> storyIDs)
        {
            // No IDs, no results
            if(storyIDs.Count == 0) { return new List<BestStoriesModel>(); }

            // Get the URL to get story details
            var apiURL = ConfigurationManager.AppSettings["HNStoryDetailsURL"];

            // Loop through our story IDs and generate a list of best stories
            var bestStores = new List<BestStoriesModel>();
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
            return bestStores;
        }
    }
}