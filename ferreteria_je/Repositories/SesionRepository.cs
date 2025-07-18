using ferreteria_je.Repositories.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace ferreteria_je.Repositories
{
    public class SesionRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public usuario ValidarCredenciales(string email, string password)
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM usuarios WHERE email = @email AND password = @password";
                return db.QueryFirstOrDefault<usuario>(sql, new { email, password });
            }
        }
    }
}
