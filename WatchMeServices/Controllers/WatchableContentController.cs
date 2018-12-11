using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WatchMeServices.Models;

namespace WatchMeServices.Controllers
{
    [RoutePrefix("api/sadrzaj")]
    public class WatchableContentController : ApiController
    {
        public SqlConnection connection = Database.DBCon.BuildConnection();

        [HttpGet]
        [Route("sadrzaj/{id:int}")]
        public IHttpActionResult GetMovieById(int id)
        {
            WatchableContent movie = new WatchableContent();
            try
            {
                

                using (connection)
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
                                movie.CoverPhoto = reader.GetString(7);
                                
                                var json = JsonConvert.SerializeObject(movie);

                                return Ok(json);

                            }
                        }
                    }
                }
                connection.Close();

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;

        }

        [HttpGet]
        [Route("sadrzaj")]
        public IHttpActionResult GetAllMovies()
        {
            WatchableContent movie = new WatchableContent();
            List<WatchableContent> listaFilmova = new List<WatchableContent>();
            try
            {
                

                using (connection)
                {

                    connection.Open();

                    string sql = "SELECT * FROM WatchableContent ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
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
                                    movie.CoverPhoto = reader.GetString(7);
                                    listaFilmova.Add(new WatchableContent()
                                    {
                                        ID = movie.ID,
                                        Name = movie.Name,
                                        ReleaseDate = movie.ReleaseDate,
                                        Season = movie.Season,
                                        Episode = movie.Episode,
                                        Duration = movie.Duration,
                                        FeedBack = movie.FeedBack,
                                        CoverPhoto = movie.CoverPhoto
                                    });
                                    

                                }
                                reader.NextResult();
                            }
                        }

                        var json = JsonConvert.SerializeObject(listaFilmova);
                        connection.Close();
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

        [HttpGet]
        [Route("sadrzaj/dohvati_kategorije")]
        public IHttpActionResult GetCategories()
        {
            Category category = new Category();
            List<Category> listaKategorija = new List<Category>();
            try
            {


                using (connection)
                {

                    connection.Open();

                    string sql = "SELECT DISTINCT * FROM Genre";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    category.ID = reader.GetInt32(0);
                                    category.Name = reader.GetString(1);
                                    listaKategorija.Add(new Category()
                                    {
                                        ID = category.ID,
                                        Name = category.Name,
                                        
                                    });


                                }
                                reader.NextResult();
                            }
                        }

                        var json = JsonConvert.SerializeObject(listaKategorija);
                        connection.Close();
                        return Ok(json);
                        
                    }

                }

            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
