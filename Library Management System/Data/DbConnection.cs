using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Library_Management_System.Data
{
    public class DbConnection
    {
        private readonly string connectionstring=
            @"Server=localhost;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionstring);
        }
    }
}
