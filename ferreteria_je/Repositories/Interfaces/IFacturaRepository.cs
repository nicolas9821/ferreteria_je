using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces; // Asegúrate de que esta sea la ruta correcta a IRepository
using Dapper; // Para usar extensiones de Dapper como QueryFirstOrDefault, Query

namespace ferreteria_je.Repositories.Interfaces
{
    public interface IFacturaRepository : IRepository<factura>
    {
        // Puedes agregar métodos específicos para Factura aquí, por ejemplo:
        // Factura GetFacturaByCodigo(string codigoFactura);
        List<factura> GetFacturasByFecha(DateTime fechaBusqueda);
        List<factura> GetFacturasWithClientName(string searchTerm = "");
    }
}