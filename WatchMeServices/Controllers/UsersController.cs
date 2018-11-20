using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WatchMeServices.Models;

namespace WatchMeServices.Controllers
{
    public class UsersController : ApiController
    {
        // GET: api/Users
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Users/5
        public Users Get(int id)
        {
            Users user = new Users();
            user.ID = id;
            user.Name = "Marko";
            user.Surname = "Vertus";
            user.email = "jodavzvz@gmail.com";
            user.Password = "Something24";
            return user;
        }

        // POST: api/Users
        public void Post([FromBody]Users value)
        {
        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
