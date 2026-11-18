using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace LibraryManagement.Core.Data
{
    public class DbConnection
    {
        private readonly string connectionstring =
            @"Server=localhost;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionstring);
        }
    }
}
