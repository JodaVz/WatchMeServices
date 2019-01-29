using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchMeServices.Models
{
    public class RatedCategoryList
    {
        public string Name { get; set; }
        public List<WatchableContentRated> MovieList { get; set; }
    }
}