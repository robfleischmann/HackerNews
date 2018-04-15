using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HackerNews.Services;
using HackerNews.Models;

namespace HackerNews.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Obtains the best stories from the Hacker News API and returns
        /// details of author and title
        /// </summary>
        /// <returns></returns>
        private async Task<List<BestStoriesModel>> getBestStories()
        {
            // Init the search text to empty
            ViewBag.newsSearchText = "";
            // Get all the best stories
            var bestStoriesIDs = await HackerNewsService.GetBestStoriesIDs();
            var bestStories = await HackerNewsService.GetBestStories(bestStoriesIDs);
            return bestStories.OrderBy(m => m.author).ToList();
        }

        public async Task<ActionResult> Index()
        {
            var bestStories = await getBestStories();
            return View(bestStories);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string newsSearchText)
        {
            // First get our stories
            var bestStories = await getBestStories();
            ViewBag.newsSearchText = "";
            // Now if we have a valid string passed in, perform a where clause
            if (newsSearchText.Length > 1)
            {
                ViewBag.newsSearchText = newsSearchText;
                var searchText = newsSearchText.Trim().ToLower();
                bestStories = bestStories.Where(m => m.author.ToLower().Contains(searchText) || m.title.ToLower().Contains(searchText)).OrderBy(m => m.author).ToList();
            }
            return View(bestStories);
        }
    }
}