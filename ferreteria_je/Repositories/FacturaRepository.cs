using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric;
using ferreteria_je.Utilidades; // Asumo que tienes una clase Log para manejar errores
using Dapper;

namespace ferreteria_je.Repositories
{
    public class FacturaRepository : GenericRepository<factura>, IFacturaRepository
    {
        public FacturaRepository()
        {
        }

        public List<factura> GetFacturasByFecha(DateTime fechaBusqueda)
        {
            List<factura> listaFacturas = new List<factura>();
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM factura WHERE DATE(fecha) = @FechaBusqueda";
                    listaFacturas = db.Query<factura>(sql, new { FechaBusqueda = fechaBusqueda.Date }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener facturas por fecha: {fechaBusqueda.ToShortDateString()}", ex);
                return new List<factura>();
            }
            return listaFacturas;
        }

        public List<factura> GetFacturasWithClientName(string searchTerm = "")
        {
            List<factura> facturas = new List<factura>();
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = @"
                    SELECT
                        id_factura,
                        fecha,
                        id_cliente,
                        total,
                        nombre_cliente
                    FROM
                        vista_facturas_con_cliente
                    WHERE
                        (@SearchTerm = '' OR nombre_cliente LIKE @SearchTerm OR id_factura LIKE @SearchTerm)
                    ORDER BY
                        fecha DESC"; // Puedes ajustar el orden según necesites

                    facturas = db.Query<factura>(sql, new { SearchTerm = $"%{searchTerm}%" }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar facturas con nombres de cliente: {ex.Message}", ex);
                // Considera lanzar una excepción o retornar un listado vacío dependiendo de tu estrategia de manejo de errores
            }
            return facturas;
        }
    }
}