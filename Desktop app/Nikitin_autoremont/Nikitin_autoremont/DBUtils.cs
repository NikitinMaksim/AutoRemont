using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Nikitin_autoremont
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "127.0.0.1";
            int port = 3306;
            string database = "autoremont";
            string username = "root";
            string password = "password";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
