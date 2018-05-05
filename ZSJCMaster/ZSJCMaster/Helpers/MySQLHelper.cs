using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSJCMaster.Helpers
{
    public class MySQLHelper
    {
        IDbConnection conn;
        public MySQLHelper(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
        }
        public IEnumerable<T> Query<T>(string sql,object param = null)
        {
            return this.conn.Query<T>(sql,param);
        }

        public int Execute(string sql,object param = null)
        {
            return this.conn.Execute(sql, param);
        }
    }
}
