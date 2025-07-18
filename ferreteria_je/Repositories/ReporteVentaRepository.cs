// ReporteVentaRepository

// ferreteria_je\Repositories\ReporteVentaRepository.cs
using System;
using MySql.Data.MySqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using ferreteria_je.Repositories.Models;
using System.Configuration;

namespace ferreteria_je.Repositories
{
    public class ReporteVentaRepository
    {
        private string connectionString;

        public ReporteVentaRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'MySqlConnectionString' no se encontró en Web.config o está vacía.");
            }
        }

        /// <summary>
        /// Obtiene una lista detallada de ventas, opcionalmente filtrada por rango de fechas, utilizando la vista 'vista_reporte_ventas'.
        /// </summary>
        public IEnumerable<ReporteVentaViewModel> GetReporteDetalle(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            using (MySqlConnection db = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT
                        IdVenta,
                        FechaVenta,
                        NombreCliente,
                        EmailCliente,
                        NombreUsuario,
                        NombreProducto,
                        CantidadVendida,
                        PrecioUnitarioVenta,
                        SubtotalLinea,
                        TotalVenta
                    FROM
                        vista_reporte_ventas
                    WHERE 1=1 ";

                var parameters = new DynamicParameters();

                if (fechaInicio.HasValue)
                {
                    query += " AND FechaVenta >= @FechaInicio";
                    parameters.Add("FechaInicio", fechaInicio.Value);
                }
                if (fechaFin.HasValue)
                {
                    query += " AND FechaVenta <= @FechaFin";
                    parameters.Add("FechaFin", fechaFin.Value.Date.AddDays(1).AddTicks(-1)); // Incluye todo el día final
                }

                query += " ORDER BY FechaVenta DESC, IdVenta DESC;";

                return db.Query<ReporteVentaViewModel>(query, parameters);
            }
        }

        /// <summary>
        /// Obtiene una lista agregada de ventas por producto con ganancias estimadas.
        /// </summary>
        public IEnumerable<ReporteVentaViewModel> GetReporteVentasPorProductoAgregado(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            using (MySqlConnection db = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT
                        p.id_producto AS IdProductoAgregado,
                        p.nombre AS NombreProductoAgregado,
                        SUM(dv.cantidad) AS CantidadTotalVendidaAgregado,
                        SUM(dv.cantidad * dv.precio_unitario) AS IngresoTotalGeneradoAgregado,
                        SUM(dv.cantidad * (SELECT COALESCE(MAX(dc.precio_unitario), 0) FROM detalle_compra dc WHERE dc.id_producto = p.id_producto)) AS CostoTotalEstimadoAgregado,
                        (SUM(dv.cantidad * dv.precio_unitario) - SUM(dv.cantidad * (SELECT COALESCE(MAX(dc.precio_unitario), 0) FROM detalle_compra dc WHERE dc.id_producto = p.id_producto))) AS GananciaEstimadaAgregado
                    FROM
                        detalle_venta dv
                    JOIN
                        venta v ON dv.id_venta = v.id_venta
                    JOIN
                        producto p ON dv.id_producto = p.id_producto
                    WHERE 1=1";

                var parameters = new DynamicParameters();

                if (fechaInicio.HasValue)
                {
                    query += " AND v.fecha >= @FechaInicio";
                    parameters.Add("@FechaInicio", fechaInicio.Value);
                }
                if (fechaFin.HasValue)
                {
                    query += " AND v.fecha <= @FechaFin";
                    parameters.Add("@FechaFin", fechaFin.Value.Date.AddDays(1).AddTicks(-1));
                }

                query += @"
                    GROUP BY
                        p.id_producto, p.nombre
                    ORDER BY
                        GananciaEstimadaAgregado DESC;";

                return db.Query<ReporteVentaViewModel>(query, parameters);
            }
        }
    }
}