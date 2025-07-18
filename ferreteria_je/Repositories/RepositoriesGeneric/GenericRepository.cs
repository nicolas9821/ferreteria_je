using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using Dapper.Contrib.Extensions;

namespace ferreteria_je.Repositories.RepositoriesGeneric
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly string connectionString;

        public GenericRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public T Get(Func<T, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                return db.GetAll<T>();
            }
        }

        public void Add(T entity)
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                db.Insert(entity);
            }
        }

        public void Update(T entity)
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                db.Update(entity);
            }
        }

        public void Delete(T entity)
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                db.Delete(entity);
            }
        }

    }
}