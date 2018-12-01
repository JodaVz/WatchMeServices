using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchMeServices.Models
{
    public class Users
    {
        private Users user;

        

        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string email { get; set; }
        public string Password { get; set; }

    }
}