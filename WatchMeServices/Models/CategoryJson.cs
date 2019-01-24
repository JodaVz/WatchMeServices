using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchMeServices.Models
{
    public class CategoryJson
    {
        public string Name { get; set; }
        public List<WatchableContent> MovieList { get; set; }
    }
}