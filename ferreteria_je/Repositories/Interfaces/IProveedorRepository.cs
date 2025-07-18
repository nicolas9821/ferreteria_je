using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ferreteria_je.Repositories.Interfaces
{
    // Hereda de tu IRepository genérica para obtener los métodos CRUD básicos
    public interface IProveedorRepository : IRepository<proveedor>
    {
        // Aquí puedes definir métodos específicos para Proveedores si los necesitas.
        // Por ejemplo, buscar un proveedor por su email.
        proveedor GetByEmail(string email);
        // O buscar proveedores por una parte de su nombre
        List<proveedor> GetProveedoresByNombre(string nombreParcial);
    }
}
// ferreteria_je.Repositories.Interfaces/IProveedorRepository.cs

