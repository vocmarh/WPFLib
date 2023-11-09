using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExport.Model
{
    public class SQLDBConnect
    {
        public static SqlConnection connect;

        public void ConnectDB()
        {
            //Connect to SQL database
            connect = new SqlConnection("Data Source=HRAMCOV;Initial Catalog=testing;Integrated Security=True");
            connect.Open();
        }

        /// <summary>
        /// Function to handle SQL command queries
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public SqlCommand Query(string sqlQuery)
        {
            SqlCommand command = new SqlCommand(sqlQuery, connect);
            return command;
        }
    }
}
