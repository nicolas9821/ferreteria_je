using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric;
using ferreteria_je.Utilidades; // Asumo que tienes una clase de utilidad para logs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;

namespace ferreteria_je.Repositories
{
    public class VentaRepository : GenericRepository<venta>, IVentaRepository
    {
        public VentaRepository()
        {
            // El constructor de GenericRepository ya se encarga de la connectionString
        }

        public venta GetVentaById(int idVenta)
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM venta WHERE id_venta = @IdVenta LIMIT 1";
                    return db.QueryFirstOrDefault<venta>(sql, new { IdVenta = idVenta });
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener venta por ID: {idVenta}", ex);
                return null;
            }
        }

        // Puedes añadir otros métodos específicos si son necesarios, como GetVentasByCliente, etc.
        public List<venta> GetVentasBySearchTerm(string searchTerm)
        {
            List<venta> listaVentas = new List<venta>();
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    // Intentamos parsear el searchTerm a un entero para buscar por ID
                    if (int.TryParse(searchTerm, out int idVenta))
                    {
                        string sql = "SELECT * FROM venta WHERE id_venta = @IdVenta";
                        listaVentas = db.Query<venta>(sql, new { IdVenta = idVenta }).ToList();
                    }
                    // Si el searchTerm no es un número o si quieres buscar por otros campos (ej. fecha si fuera string)
                    // puedes añadir más lógica aquí. Por ahora, solo por ID.
                    // string sql = "SELECT * FROM venta WHERE CONVERT(id_venta, CHAR) LIKE @SearchTerm OR DATE_FORMAT(fecha, '%Y-%m-%d') LIKE @SearchTerm";
                    // listaVentas = db.Query<venta>(sql, new { SearchTerm = $"%{searchTerm}%" }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener ventas por término de búsqueda: {searchTerm}", ex);
                return new List<venta>();
            }
            return listaVentas;
        }
    }
}