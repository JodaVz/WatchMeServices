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

                    string sql = "SELECT TOP 5 WatchableContent.* FROM WatchableContent,Feedback,Users WHERE LeftBy = Users.ID AND CommentedOn = WatchableContent.ID GROUP BY WatchableContent.ID,WatchableContent.Name,WatchableContent.ReleasedDate,WatchableContent.Season,WatchableContent.Episode,WatchableContent.Duration,WatchableContent.Feedback,WatchableContent.CoverPhoto ORDER BY SUM(Rating) DESC; ";

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

        [HttpGet]
        [Route("sadrzaj/dohvati_prema_kategoriji")]
        public IHttpActionResult GetMoviesByCategory()
        {
            
            List<string> listaZanrova = new List<string>();
            List<List<WatchableContentCategory>> listaPoKategorijama = new List<List<WatchableContentCategory>>();
           
            try
            {


                using (connection)
                {

                    connection.Open();
                    //Upit za dobivanje svih imena zanrova za koje postoji neka zavisnost ( odnosno, postoje filmovi koji spadaju u taj zanr)
                    string sql1 = "SELECT DISTINCT Name FROM Genre, IsGenre WHERE IsGenre.Genre_ID = Genre.ID";
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    listaZanrova.Add(new string(reader.GetString(0).ToCharArray()));
                                }
                                reader.NextResult();
                            }
                            // var jsontest = JsonConvert.SerializeObject(listaZanrova);
                            // return Ok(jsontest);
                        }
                    }


                   
                    foreach (var zanr in listaZanrova)
                    {
                        
                        string sql = "SELECT odgovor.* FROM(SELECT WatchableContent.*,Genre.Name as " +
                       "NazivZanra FROM WatchableContent,Genre,IsGenre WHERE WatchableContent.ID=IsGenre.Movie_ID AND Genre.ID=IsGenre.Genre_ID)" +
                       " odgovor WHERE odgovor.NazivZanra='"+zanr+"';";
                        List<WatchableContentCategory> listawcC = new List<WatchableContentCategory>();
                        WatchableContentCategory wCC = new WatchableContentCategory();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        wCC.ID = reader.GetInt32(0);
                                        wCC.Name = reader.GetString(1);
                                        wCC.ReleaseDate = reader.GetDateTime(2);
                                        wCC.Season = reader.GetInt32(3);
                                        wCC.Episode = reader.GetInt32(4);
                                        wCC.Duration = Convert.ToDouble(reader.GetValue(5));
                                        wCC.FeedBack = reader.GetInt32(6);
                                        wCC.CoverPhoto = reader.GetString(7);
                                        wCC.Category = reader.GetString(8);
                                        listawcC.Add(new WatchableContentCategory()
                                        {
                                            ID = wCC.ID,
                                            Name = wCC.Name,
                                            ReleaseDate = wCC.ReleaseDate,
                                            Season = wCC.Season,
                                            Episode = wCC.Episode,
                                            Duration = wCC.Duration,
                                            FeedBack = wCC.FeedBack,
                                            CoverPhoto = wCC.CoverPhoto,
                                            Category=wCC.Category
                                        });


                                    }
                                    reader.NextResult();
                                }
                            }
                            listaPoKategorijama.Add(new List<WatchableContentCategory>(listawcC));
                            listawcC.Clear();

                        }
                        //kraj foreacha
                    }

                    var json = JsonConvert.SerializeObject(listaPoKategorijama);
                    connection.Close();
                    return Ok(json);
                }
                

            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("sadrzaj/dohvati_kategorije_filmova")]
        public IHttpActionResult GetCategoryMovies()
        {
            CategoryJson category = new CategoryJson();
            List<RatedCategoryList> listaKategorija = new List<RatedCategoryList>();
            WatchableContentRated film = new WatchableContentRated();
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
                                    
                                    category.Name = reader.GetString(1);
                                    listaKategorija.Add(new RatedCategoryList()
                                    {
                                        
                                        Name = category.Name,
                                        MovieList= new List<WatchableContentRated>()
                                        

                                    });


                                }
                                reader.NextResult();
                            }
                        }

                        //var json = JsonConvert.SerializeObject(listaKategorija);
                      
                       

                    }
                    foreach (RatedCategoryList item in listaKategorija)
                    {
                         sql = "select WatchableContent.*,SUM(f.Rating) As UkupniRating from WatchableContent left join Feedback f on WatchableContent.ID = f.CommentedOn left join IsGenre isg on WatchableContent.ID = isg.Movie_ID left join Genre g on g.ID = isg.Genre_ID where WatchableContent.ID = isg.Movie_ID AND g.ID = isg.Genre_ID AND g.Name = '"+item.Name+"' GROUP BY  WatchableContent.ID,WatchableContent.Name,WatchableContent.Name,WatchableContent.ReleasedDate,WatchableContent.Season,WatchableContent.Episode,WatchableContent.Duration,WatchableContent.Feedback,WatchableContent.CoverPhoto;";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        film.ID = reader.GetInt32(0);
                                        film.Name = reader.GetString(1);
                                        film.ReleaseDate = reader.GetDateTime(2);
                                        film.Season = reader.GetInt32(3);
                                        film.Episode = reader.GetInt32(4);
                                        film.Duration = Convert.ToDouble(reader.GetValue(5));
                                        film.FeedBack = reader.GetInt32(6);
                                        film.CoverPhoto = reader.GetString(7);
                                        if(!reader.IsDBNull(8)) film.Rating = reader.GetInt32(8);
                                        if (reader.IsDBNull(8)) film.Rating = 0;
                                        item.MovieList.Add(new WatchableContentRated()
                                        {
                                            ID=film.ID,
                                            Name=film.Name,
                                            ReleaseDate=film.ReleaseDate,
                                            Season=film.Season,
                                            Episode=film.Episode,
                                            Duration=film.Duration,
                                            FeedBack=film.FeedBack,
                                            CoverPhoto=film.CoverPhoto,
                                            Rating=film.Rating
                                            
                                            

                                        });


                                    }
                                    reader.NextResult();
                                }
                            }

                           



                        }
                    }
                    var json = JsonConvert.SerializeObject(listaKategorija);
                    connection.Close();
                    return Ok(json);

                }

            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
