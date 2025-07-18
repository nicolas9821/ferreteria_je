// IClienteRepository.cs

using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ferreteria_je.Repositories.Interfaces
{
    public interface IClienteRepository : IRepository<cliente>
    {
        // Aquí puedes definir métodos específicos para Clientes.
        // Siguiendo el ejemplo de ProveedorRepository, podrías tener:

        /// <summary>
        /// Obtiene un cliente por su número de cédula.
        /// </summary>
        /// <param name="cedula">El número de cédula del cliente.</param>
        /// <returns>El objeto cliente si se encuentra, de lo contrario, null.</returns>
        cliente GetByCedula(string cedula);

        /// <summary>
        /// Obtiene una lista de clientes cuyo nombre, cédula, teléfono o email contenga un término de búsqueda.
        /// </summary>
        /// <param name="searchTerm">El término a buscar en los campos del cliente.</param>
        /// <returns>Una lista de objetos cliente que coinciden con el término de búsqueda.</returns>
        List<cliente> GetClientesBySearchTerm(string searchTerm);
    }

}




