using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWebPages.Models;

namespace TestWebPages.Controllers
{
    public class HomeController : Controller
    {
        Dictionary<int, List<int>> UrlIdMapping;
        Dictionary<int, int> WordIdMapping;
        string TestWord;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test(int id)
        {
            InitializeData();
            TestModel model = new TestModel();
            model.Id = id;
            model.TestWord = this.TestWord;
            model.TestWordCount = WordIdMapping[id];
            if (UrlIdMapping.Keys.Contains(id)) model.PageUrls = UrlIdMapping[id];
            return View(model);
        }

        [NonAction]
        public void InitializeData()
        {
            if (UrlIdMapping != null) return;
            UrlIdMapping = new Dictionary<int, List<int>>();
            UrlIdMapping.Add(1, new List<int> { 2, 3, 4, 5, 6, 7, 8 });
            UrlIdMapping.Add(2, new List<int> { 9 });
            UrlIdMapping.Add(3, new List<int> { 9 });
            UrlIdMapping.Add(6, new List<int> { 10 });
            UrlIdMapping.Add(8, new List<int> { 11 });
            UrlIdMapping.Add(10, new List<int> { 12, 13 });
            WordIdMapping = new Dictionary<int, int>();
            WordIdMapping.Add(1, 5);
            WordIdMapping.Add(2, 1);
            WordIdMapping.Add(3, 1);
            WordIdMapping.Add(4, 1);
            WordIdMapping.Add(5, 1);
            WordIdMapping.Add(6, 1);
            WordIdMapping.Add(8, 1);
            WordIdMapping.Add(7, 1);
            WordIdMapping.Add(9, 5);
            WordIdMapping.Add(10, 5);
            WordIdMapping.Add(11, 5);
            WordIdMapping.Add(12, 2);
            WordIdMapping.Add(13, 2);
            TestWord = "null";
        }

    }
}