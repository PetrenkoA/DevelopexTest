using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebPages.Models
{
    public class TestModel
    {
        public int Id { get; set; }

        public List<int> PageUrls { get; set; }

        public string TestWord { get; set; }

        public int TestWordCount { get; set; }
    }
}