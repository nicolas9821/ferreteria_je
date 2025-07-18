using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using System.Collections.Generic;

namespace ferreteria_je.Repositories.Interfaces
{
    // Extends the generic IRepository for the 'producto' model
    public interface IProductoRepository : IRepository<producto>
    {
        // Specific method to find products by a partial name match
        List<producto> GetProductosByNombre(string nombreParcial);

        // You might also want a method to get a product by its exact name, similar to GetByEmail for Proveedor
        producto GetByNombre(string nombre);
        List<producto> GetProductosBySearchTerm(string searchTerm);
    }
}

