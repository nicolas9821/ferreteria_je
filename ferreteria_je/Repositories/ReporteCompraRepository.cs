// ReporteCompraRepository

// ferreteria_je\Repositories\ReporteCompraRepository.cs
using Dapper;
using ferreteria_je.Repositories.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace ferreteria_je.Repositories
{
    public class ReporteCompraRepository
    {
        private string connectionString;

        public ReporteCompraRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'MySqlConnectionString' no se encontró en Web.config o está vacía.");
            }
        }

        /// <summary>
        /// Obtiene una lista detallada de compras, opcionalmente filtrada por rango de fechas, utilizando la vista 'vista_reporte_compras_detalle'.
        /// </summary>
        public IEnumerable<ReporteCompraViewModel> GetReporteDetalle(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            using (MySqlConnection db = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT
                        IdCompra,
                        FechaCompra,
                        NombreProveedor,
                        EmailProveedor,
                        NombreUsuario,
                        NombreProducto,
                        CantidadComprada,
                        PrecioUnitarioCompra,
                        SubtotalLinea,
                        TotalCompra
                    FROM
                        vista_reporte_compras_detalle
                    WHERE 1=1 ";

                var parameters = new DynamicParameters();

                if (fechaInicio.HasValue)
                {
                    query += " AND FechaCompra >= @FechaInicio";
                    parameters.Add("FechaInicio", fechaInicio.Value);
                }
                if (fechaFin.HasValue)
                {
                    query += " AND FechaCompra <= @FechaFin";
                    parameters.Add("FechaFin", fechaFin.Value.Date.AddDays(1).AddTicks(-1));
                }

                query += " ORDER BY FechaCompra DESC, IdCompra DESC;";

                return db.Query<ReporteCompraViewModel>(query, parameters);
            }
        }

        /// <summary>
        /// Obtiene una lista agregada de compras por proveedor.
        /// </summary>
        public IEnumerable<ReporteCompraViewModel> GetReporteComprasPorProveedorAgregado(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            using (MySqlConnection db = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT
                        p.id_proveedor AS IdProveedorAgregado,
                        p.nombre AS NombreProveedorAgregado,
                        SUM(c.total) AS TotalComprasAgregado
                    FROM
                        compra c
                    JOIN
                        proveedor p ON c.id_proveedor = p.id_proveedor
                    WHERE 1=1";

                var parameters = new DynamicParameters();

                if (fechaInicio.HasValue)
                {
                    query += " AND c.fecha >= @FechaInicio";
                    parameters.Add("@FechaInicio", fechaInicio.Value);
                }
                if (fechaFin.HasValue)
                {
                    query += " AND c.fecha <= @FechaFin";
                    parameters.Add("@FechaFin", fechaFin.Value.Date.AddDays(1).AddTicks(-1));
                }

                query += @"
                    GROUP BY
                        p.id_proveedor, p.nombre
                    ORDER BY
                        TotalComprasAgregado DESC;";

                return db.Query<ReporteCompraViewModel>(query, parameters);
            }
        }
    }
}