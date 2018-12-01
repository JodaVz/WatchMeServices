using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WatchMeServices.Models;

namespace WatchMeServices.Controllers
{
    [RoutePrefix("api/sadrzaj")]
    public class WatchableContentController : ApiController
    {
        [HttpGet]
        [Route("sadrzaj/{id:int}")]
        public IHttpActionResult GetMovieById(int id)
        {
            WatchableContent movie = new WatchableContent();
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

                    string sql = "SELECT * FROM WatchableContent WHERE ID=" + id;

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                movie.ID = reader.GetInt32(0);
                                movie.Name = reader.GetString(1);
                                movie.ReleaseDate = reader.GetDateTime(2);
                                movie.Season = reader.GetInt32(3);
                                movie.Episode = reader.GetInt32(4);
                                movie.Duration = Convert.ToDouble(reader.GetValue(5));
                                movie.FeedBack = reader.GetInt32(6);
                                movie.CoverPhoto = reader.GetStream(7).ToString();
                                
                                var json = JsonConvert.SerializeObject(movie);

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
    }
}
