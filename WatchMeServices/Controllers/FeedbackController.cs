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
                //string comment = feedback.Comments;
                int leftby = feedback.LeftBy;
                int commentedon = feedback.CommentedOn;
                int rating = feedback.Rating;
                //Comment = comment;
                LeftBy = leftby;
                CommentedOn = commentedon;
                Rating = rating;
            }
            catch (Exception e)
            {

                throw e;
            }

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
                            }
                        }
                    }
                    connection.Close();
                    if (rating==1)
                    {
                        return Ok(); //ako je pozitivno ocjenjeno onda je 200 OK
                    }
                    if (rating==0)
                    {
                        return BadRequest(); //ako je negativno ocjenjeno onda je BadRequest 400
                    }
                    else
                    {
                        return NotFound(); //ako smije likeat i dislikat onda je NotFound 404
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
