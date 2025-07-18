using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric;
using Dapper; // Importar Dapper para consultas SQL directas

namespace ferreteria_je.Repositories.Inicio // Nuevo namespace para los repositorios del dashboard
{
    public class InicioVentaRepository : GenericRepository<venta>
    {
        public InicioVentaRepository() : base()
        {
        }

        /// <summary>
        /// Obtiene el total de ventas para el día actual.
        /// </summary>
        /// <returns>El monto total de ventas del día.</returns>
        public decimal GetVentasHoy()
        {
            decimal totalVentas = 0;
            string query = "SELECT SUM(total) FROM venta WHERE DATE(fecha) = CURDATE()";

            using (var db = new MySqlConnection(connectionString))
            {
                totalVentas = db.ExecuteScalar<decimal>(query);
            }
            return totalVentas;
        }

        /// <summary>
        /// DTO (Data Transfer Object) para los últimos pedidos.
        /// Se usa porque el modelo 'venta' no tiene directamente el 'nombre_cliente',
        /// y se obtiene mediante una unión en la consulta.
        /// </summary>
        public class LatestOrderDto
        {
            public int id_venta { get; set; }
            public DateTime fecha { get; set; }
            public decimal total_venta { get; set; }
            public string nombre_cliente { get; set; }
        }

        /// <summary>
        /// Obtiene una lista de los últimos pedidos registrados, incluyendo el nombre del cliente.
        /// </summary>
        /// <param name="limit">Número máximo de pedidos a retornar.</param>
        /// <returns>Lista de LatestOrderDto con los detalles de los pedidos.</returns>
        public List<LatestOrderDto> GetLatestVentas(int limit)
        {
            List<LatestOrderDto> ventas = new List<LatestOrderDto>();
            string query = @"SELECT v.id_venta, v.fecha, v.total AS total_venta, c.nombre AS nombre_cliente
                             FROM venta v
                             JOIN cliente c ON v.id_cliente = c.id_cliente
                             ORDER BY v.fecha DESC LIMIT @limit";

            using (var db = new MySqlConnection(connectionString))
            {
                ventas = db.Query<LatestOrderDto>(query, new { limit }).AsList();
            }
            return ventas;
        }

        // El método GetVentasMensualesPorDia ha sido eliminado ya que la gráfica de tendencia de ventas se ha quitado.
    }
}
