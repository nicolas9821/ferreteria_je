using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric;
using ferreteria_je.Utilidades; // Asegúrate de tener esta utilidad para el log

namespace ferreteria_je.Repositories
{
    public class CompraRepository : GenericRepository<compra>, ICompraRepository
    {
        public CompraRepository()
        {
        }

        public IEnumerable<compra> GetComprasByFecha(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM compra WHERE 1=1";
                    if (fechaInicio.HasValue)
                    {
                        sql += " AND fecha >= @FechaInicio";
                    }
                    if (fechaFin.HasValue)
                    {
                        sql += " AND fecha <= @FechaFin";
                    }
                    return db.Query<compra>(sql, new { FechaInicio = fechaInicio, FechaFin = fechaFin }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener compras por rango de fecha: {ex.Message}", ex);
                return new List<compra>();
            }
        }

        // ***** MÉTODO MODIFICADO PARA INCLUIR EL NOMBRE DEL USUARIO *****
        public IEnumerable<compra> GetComprasWithProveedorName(string searchTerm = "")
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = @"
                        SELECT
                            c.*,  -- Selecciona todas las columnas de la tabla compra
                            p.nombre AS nombre_proveedor, -- Alias para el nombre del proveedor
                            u.nombre AS nombre_usuario    -- Alias para el nombre del usuario (columna 'nombre' en tabla 'usuarios')
                        FROM
                            compra c
                        LEFT JOIN
                            proveedor p ON c.id_proveedor = p.id_proveedor
                        LEFT JOIN
                            usuarios u ON c.id_usuario = u.id_usuario -- ¡NUEVO JOIN para la tabla de usuarios!
                        WHERE 1=1";

                    string orderByClause = " ORDER BY c.fecha DESC, c.id_compra DESC"; // Ordenar para una vista consistente

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        searchTerm = $"%{searchTerm.Trim()}%"; // Añadir comodines para búsqueda LIKE
                        // Buscar por ID de compra, fecha (parcial), nombre de proveedor O nombre de usuario
                        sql += @" AND (
                                    CAST(c.id_compra AS CHAR) LIKE @SearchTerm OR
                                    DATE_FORMAT(c.fecha, '%Y-%m-%d') LIKE @SearchTerm OR
                                    p.nombre LIKE @SearchTerm OR
                                    u.nombre LIKE @SearchTerm -- ¡NUEVO CRITERIO DE BÚSQUEDA!
                                )";
                    }

                    sql += orderByClause; // Añade la cláusula ORDER BY al final

                    // Dapper mapeará los resultados a la clase 'compra',
                    // poblando las propiedades nombre_proveedor y nombre_usuario (marcadas con [Write(false)])
                    return db.Query<compra>(sql, new { SearchTerm = searchTerm }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener compras con nombre de proveedor y usuario: {ex.Message}", ex);
                return new List<compra>();
            }
        }
        // ***** FIN DE MÉTODO MODIFICADO *****        }
    }
}