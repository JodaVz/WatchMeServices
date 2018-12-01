using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchMeServices.Models
{
    public class WatchableContent
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public double Duration { get; set; }
        public int FeedBack { get; set; }
        public string CoverPhoto { get; set; }

    }
}