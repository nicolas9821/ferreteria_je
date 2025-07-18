using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric;
using Dapper; // Importar Dapper para consultas SQL directas

namespace ferreteria_je.Repositories.Inicio // Nuevo namespace para los repositorios del dashboard
{
    public class InicioProductoRepository : GenericRepository<producto>
    {
        public InicioProductoRepository() : base()
        {
        }

        /// <summary>
        /// Obtiene la cantidad de productos cuyo stock está por debajo de un umbral mínimo.
        /// </summary>
        /// <param name="umbralMinimo">El valor de stock que define el "stock bajo".</param>
        /// <returns>El número de productos con stock bajo.</returns>
        public int GetCantidadProductosBajoStock(int umbralMinimo)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM producto WHERE stock < @umbralMinimo";

            using (var db = new MySqlConnection(connectionString))
            {
                count = db.ExecuteScalar<int>(query, new { umbralMinimo });
            }
            return count;
        }

        /// <summary>
        /// Obtiene una lista de productos cuyo stock está por debajo de un umbral mínimo.
        /// </summary>
        /// <param name="umbralMinimo">El valor de stock que define el "stock bajo".</param>
        /// <returns>Lista de objetos 'producto' con stock bajo.</returns>
        public List<producto> GetProductosBajoStock(int umbralMinimo)
        {
            List<producto> productos = new List<producto>();
            string query = "SELECT id_producto, nombre, stock FROM producto WHERE stock < @umbralMinimo ORDER BY stock ASC";

            using (var db = new MySqlConnection(connectionString))
            {
                productos = db.Query<producto>(query, new { umbralMinimo }).AsList();
            }
            return productos;
        }

        // El método GetProductosMasVendidos ha sido eliminado ya que la gráfica de productos más vendidos se ha quitado.
    }
}
