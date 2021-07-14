using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner.DataLayer
{
    class OwnSqlServerConnectionFactory : IOwnConnectionFactory
    {
        string ConnectionString { get; set; }
        public OwnSqlServerConnectionFactory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public DbConnection CreateConnection()
        {
            DbConnection dbConnection = new SqlConnection(this.ConnectionString);
            return dbConnection;             
        }
    }
}
