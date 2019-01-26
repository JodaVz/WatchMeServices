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

    [RoutePrefix("api/feedback")]
    public class FeedbackController : ApiController
    {
        public static string Comment;
        public static int LeftBy;
        public static int CommentedOn;
        public static int Rating;
        public SqlConnection connection = Database.DBCon.BuildConnection();

        [HttpPost]
        [Route("feedback/spremi_ocjenu")]
        public void SaveMovieFeedback([FromBody]string dobiveniPodaci)
        {
            try
            {
                string podaci= dobiveniPodaci;
                Feedback feedback = JsonConvert.DeserializeObject<Feedback>(podaci);
                int leftby = feedback.LeftBy;
                int commentedon = feedback.CommentedOn;
                int rating = feedback.Rating;              
                LeftBy = leftby;
                CommentedOn = commentedon;
                Rating = rating;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        [HttpPost]
        [Route("feedback/spremi_komentar")]
        public void SaveMovieComment([FromBody]string dobiveniPodaci)
        {
            try
            {
                string podaci = dobiveniPodaci;
                Feedback feedback = JsonConvert.DeserializeObject<Feedback>(podaci);
                string comment = feedback.Comments;
                int leftby = feedback.LeftBy;
                int commentedon = feedback.CommentedOn;
                Comment = comment;
                LeftBy = leftby;
                CommentedOn = commentedon;
                
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        [HttpGet]
        [Route("feedback/spremi_komentar")]
        public IHttpActionResult DodajKomentar()
        {

            try
            {


                using (connection)
                {

                    connection.Open();

                    string sql = "IF EXISTS(SELECT * FROM Feedback WHERE LeftBy ="+LeftBy+" AND CommentedOn="+CommentedOn+")UPDATE Feedback SET Comments = '"+Comment+"' WHERE LeftBy = "+LeftBy+" AND CommentedOn = "+CommentedOn+" ELSE INSERT INTO Feedback(Comments, LeftBy, CommentedOn) VALUES('"+Comment+"', "+LeftBy+", "+CommentedOn+"); ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                    }
                    connection.Close();
                    return Ok();

                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;

        }

        [HttpGet]
        [Route("feedback/spremi_ocjenu")]
        public IHttpActionResult DodajRating()
        {

            try
            {


                using (connection)
                {

                    connection.Open();

                    string sql = "INSERT INTO Feedback(LeftBy,CommentedOn,Rating) VALUES("+LeftBy+","+CommentedOn+","+Rating+");";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                    }
                    connection.Close();
                    return Ok();
                    
                }

            }
            catch (SqlException e)
            {
                throw e;
            }
            return null;

        }

        [HttpGet]
        [Route("feedback/provjeri_ocjenu")]
        public IHttpActionResult ProvjeriRating()
        {
            int rating = 0;
            
            try
            {


                using (connection)
                {

                    connection.Open();

                    string sql = "SELECT Feedback.Rating FROM Feedback,WatchableContent,Users WHERE LeftBy = Users.ID AND CommentedOn = WatchableContent.ID and Users.ID = "+LeftBy+" AND WatchableContent.ID = "+CommentedOn+"; ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    rating = reader.GetInt32(0);
                                }
                                reader.NextResult();
                            }
                        }
                    }
                    connection.Close();
                    if (rating==1)
                    {
                        return Ok(); //ako je pozitivno ocjenjeno onda je 200 OK
                    }
                    if (rating==2)
                    {
                        return InternalServerError(); //ako je negativno ocjenjeno onda je InternalServerErro 500
                    }
                    else
                    {
                        return Unauthorized(); //ako smije likeat i dislikat onda je Unauthorized 401
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
        [Route("feedback/dohvati_komentare")]
        public IHttpActionResult GetMovieComments()
        {
            string zaUpis;
            List<string> listakomentara = new List<string>();
            try
            {


                using (connection)
                {

                    connection.Open();

                    string sql = "SELECT Feedback.Comments FROM Feedback,WatchableContent WHERE CommentedOn=WatchableContent.ID AND WatchableContent.ID="+CommentedOn+";";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    zaUpis = reader.GetString(0);

                                    listakomentara.Add(zaUpis);


                                }
                                reader.NextResult();
                            }
                        }

                        var json = JsonConvert.SerializeObject(listakomentara);
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
