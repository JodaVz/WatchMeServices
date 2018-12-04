using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WatchMeServices.Database
{
    public class DBCon
    {
        public static SqlConnection BuildConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "air2018watchme.database.windows.net";
            builder.UserID = "larisa.borovec";
            builder.Password = "Something24";
            builder.InitialCatalog = "AIR2018WatchMe";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            return connection;

        }

    }
}