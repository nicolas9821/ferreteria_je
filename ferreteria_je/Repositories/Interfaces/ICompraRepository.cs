using System;
using System.Collections.Generic;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;

namespace ferreteria_je.Repositories.Interfaces
{
    public interface ICompraRepository : IRepository<compra>
    {
        // Puedes añadir métodos específicos para Compra aquí, por ejemplo:
        IEnumerable<compra> GetComprasByFecha(DateTime? fechaInicio, DateTime? fechaFin);
        IEnumerable<compra> GetComprasWithProveedorName(string searchTerm = "");
    }
}