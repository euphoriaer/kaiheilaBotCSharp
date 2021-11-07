using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CsharpBot
{
    public static class MysqlHelper
    {
        private static MySqlConnection MyConnection;
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="connectStr"></param>
        public static void Connect(string connectStr)
        {
            string MyConString = connectStr;
            //Connect to MySQL using Connector/ODBC
            MySqlConnection MyConnection = new MySqlConnection(MyConString);
            MyConnection.Open();
           
        }

        public static void Insert(string sql)
        {
            if (MyConnection==null)
            {
                return;
            }
            MySqlCommand cmd = new MySqlCommand(sql, MyConnection);
            MyConnection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
        }

        public static void Search()
        {

        }
        public static void Delete()
        {

        }
        
    }
}