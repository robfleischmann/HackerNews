using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HackerNews.Services;

namespace HackerNews.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var bestStoriesIDs = await HackerNewsService.GetBestStoriesIDs();
            var bestStories = await HackerNewsService.GetBestStories(bestStoriesIDs);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}