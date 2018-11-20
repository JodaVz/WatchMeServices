using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WatchMeServices
{
    public static class WebApiConfig
    {
        public static MySqlConnection Conn()
        {
            string conn_string = "Server=tcp:air2018watchme.database.windows.net,1433;Initial Catalog=AIR2018WatchMe;Persist Security Info=False;User ID={larisa.borovec@gmail.com};Password={Something24};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            MySqlConnection conn = new MySqlConnection(conn_string);
            return conn;
        }
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}
