using System;
using System.Collections.Generic;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;

namespace ferreteria_je.Repositories.Interfaces
{
    public interface IVentaRepository : IRepository<venta>
    {
        // Puedes agregar métodos específicos para ventas aquí, por ejemplo:
        // List<venta> GetVentasByFecha(DateTime fecha);
        // List<venta> GetVentasByCliente(int idCliente);
        venta GetVentaById(int idVenta);
        List<venta> GetVentasBySearchTerm(string searchTerm);

    }
}