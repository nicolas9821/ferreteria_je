using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace ferreteria_je.Repositories.RepositoriesGeneric.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T Get(Func<T, bool> predicate);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
    
}
