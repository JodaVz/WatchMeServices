using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WatchMeServices.Models;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WatchMeServices.Controllers
{
    [RoutePrefix("api/korisnici")]
    public class UsersController : ApiController
    {

        public static string Email;
        public static string Password;


        // GET: api/Users/5
        /// <summary>
        /// Metoda koja vraca dohvaca korisnika iz baze prema prosljeđenom IDu, serializira sve njegove vrijednosti u JSON format i vraca ga kao odgovor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/{id:int}")]
        public IHttpActionResult GetUserById(int id)
        {
            Users user = new Users();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "air2018watchme.database.windows.net";
                builder.UserID = "larisa.borovec";
                builder.Password = "Something24";
                builder.InitialCatalog = "AIR2018WatchMe";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    
                    connection.Open();

                    string sql = "SELECT * FROM Users WHERE ID="+id;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Surname = reader.GetString(2);
                                user.email = reader.GetString(3);
                                user.Password = reader.GetString(4);
                                var json = JsonConvert.SerializeObject(user);
                               
                                return Ok(json);
                                
                            }
                        }
                    }
                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;
            
        }

        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetAllUsers()
        {
            Users user = new Users();
            List<Users> listaKorisnika = new List<Users>();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "air2018watchme.database.windows.net";
                builder.UserID = "larisa.borovec";
                builder.Password = "Something24";
                builder.InitialCatalog = "AIR2018WatchMe";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {

                    connection.Open();

                    string sql = "SELECT * FROM Users";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {


                                while (reader.Read())
                                {
                                    user.ID = reader.GetInt32(0);
                                    user.Name = reader.GetString(1);
                                    user.Surname = reader.GetString(2);
                                    user.email = reader.GetString(3);
                                    user.Password = reader.GetString(4);

                                    //var json = JsonConvert.SerializeObject(user);

                                    //return Ok(json);

                                    listaKorisnika.Add(new Users()
                                    {
                                        ID = user.ID,
                                        Name = user.Name,
                                        Surname = user.Surname,
                                        email = user.email,
                                        Password = user.Password

                                    });
                                }
                               
                                reader.NextResult();
                            }
                        }
                        var json = JsonConvert.SerializeObject(listaKorisnika);
                        return Ok(json);
                    }
                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;

        }

        /// <summary>
        /// Dodavanje novog korisnika 
        /// </summary>
        /// <param name="tekst"></param>
        /// treba dodati rad s bazom, i insert
        // POST: api/Users
        //public void Post([FromBody]string tekst)
        //{
        //    try
        //    {
        //        string proba = tekst;
        //        Users korisnik = JsonConvert.DeserializeObject<Users>(tekst);
        //        int id = korisnik.ID;
        //        string ime = korisnik.Name;
        //        string prezime = korisnik.Surname;
        //        string email = korisnik.email;
        //        string password = korisnik.Password;
               
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
           
        //}

        [HttpPost]
        [Route("users/provjeri")]
        public void SaveName([FromBody]string UsrPass)
        {
            try
            {
                string dobivenString = UsrPass;
                Users user = JsonConvert.DeserializeObject<Users>(dobivenString);
                string email = user.email;
                string pass = user.Password;
                Email = email;
                Password = pass;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        [HttpGet]
        [Route("users/provjeri")]
        public IHttpActionResult ProvjeriKorisnika()
        {
            Users user = new Users();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "air2018watchme.database.windows.net";
                builder.UserID = "larisa.borovec";
                builder.Password = "Something24";
                builder.InitialCatalog = "AIR2018WatchMe";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {

                    connection.Open();

                    string sql = "SELECT * FROM Users WHERE Email=" + "'" + Email + "'" +  " AND "+ "Password=" + "'" +Password+"'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows) return NotFound();//204 Ne postoji
                            if (reader.HasRows) return Ok();//100 sve je okej, user postoji 
                        }
                    }
                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;

        }
    }
}
