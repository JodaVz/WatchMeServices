using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchMeServices.Models
{
    public class Feedback
    {
        public int ID { get; set; }
        public string Comments { get; set; }
        public int LeftBy { get; set; }
        public int CommentedOn { get; set; }
        public int Rating { get; set; }
    }
}