using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric; // Assuming GenericRepository is here
using ferreteria_je.Utilidades; // Assuming you have a Log class here.
using Dapper; // For direct SQL queries with Dapper

namespace ferreteria_je.Repositories
{
    public class ProductoRepository : GenericRepository<producto>, IProductoRepository
    {
        public ProductoRepository()
        {
            // El connectionString es heredado de GenericRepository.
            // Asegúrate de que GenericRepository lo inicialice correctamente.
        }

        // --- Método para insertar producto utilizando el procedimiento almacenado (Parámetro IN) ---
        /// <summary>
        /// Inserta un nuevo producto en la base de datos utilizando el procedimiento almacenado 'InsertarNuevoProducto'.
        /// Este método utiliza parámetros IN para pasar los datos al SP.
        /// </summary>
        /// <param name="nombre">Nombre del producto.</param>
        /// <param name="descripcion">Descripción del producto.</param>
        /// <param name="precio">Precio del producto.</param>
        /// <param name="stock">Stock inicial del producto.</param>
        public void InsertarProductoViaSP(string nombre, string descripcion, decimal precio, int stock)
        {
            string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(currentConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("InsertarNuevoProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Indica que es un procedimiento almacenado

                    cmd.Parameters.AddWithValue("p_nombre", nombre);
                    cmd.Parameters.AddWithValue("p_descripcion", descripcion);
                    cmd.Parameters.AddWithValue("p_precio", precio);
                    cmd.Parameters.AddWithValue("p_stock", stock);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Log.Escribir($"Error de base de datos al insertar producto vía SP: {ex.Message}", ex);
                        throw new Exception("Error al insertar el producto en la base de datos. Consulte los logs para más detalles.", ex);
                    }
                    catch (Exception ex)
                    {
                        Log.Escribir($"Error inesperado al insertar producto vía SP: {ex.Message}", ex);
                        throw new Exception("Error inesperado al insertar el producto. Consulte los logs para más detalles.", ex);
                    }
                }
            }
        }

        // --- Método para obtener stock utilizando el procedimiento almacenado (Parámetro OUT) ---
        /// <summary>
        /// Obtiene el stock actual de un producto utilizando el procedimiento almacenado 'ObtenerStockProducto'.
        /// Este método utiliza un parámetro OUT para recuperar el stock.
        /// </summary>
        /// <param name="idProducto">El ID del producto cuyo stock se desea obtener.</param>
        /// <returns>El stock actual del producto, o -1 si el producto no se encuentra o hay un error.</returns>
        public int ObtenerStockProductoViaSP(int idProducto)
        {
            string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            int stockActual = -1; // Valor por defecto en caso de no encontrar o error

            using (MySqlConnection conn = new MySqlConnection(currentConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("ObtenerStockProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Indica que es un procedimiento almacenado

                    cmd.Parameters.AddWithValue("p_id_producto", idProducto);

                    MySqlParameter stockParam = new MySqlParameter("p_stock_actual", MySqlDbType.Int32);
                    stockParam.Direction = ParameterDirection.Output; // Establece la dirección como OUT
                    cmd.Parameters.Add(stockParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (stockParam.Value != DBNull.Value)
                        {
                            stockActual = Convert.ToInt32(stockParam.Value);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Log.Escribir($"Error de base de datos al obtener stock vía SP para producto ID {idProducto}: {ex.Message}", ex);
                        throw new Exception("Error al obtener el stock del producto desde la base de datos. Consulte los logs para más detalles.", ex);
                    }
                    catch (Exception ex)
                    {
                        Log.Escribir($"Error inesperado al obtener stock vía SP para producto ID {idProducto}: {ex.Message}", ex);
                        throw new Exception("Error inesperado al obtener el stock del producto. Consulte los logs para más detalles.", ex);
                    }
                }
            }
            return stockActual;
        }

        // --- Nuevo método para actualizar stock utilizando el procedimiento almacenado (Parámetro INOUT) ---
        /// <summary>
        /// Actualiza el stock de un producto en la base de datos utilizando el procedimiento almacenado 'ActualizarStockProducto'.
        /// Este método utiliza un parámetro INOUT para enviar el ajuste y recibir el nuevo stock.
        /// </summary>
        /// <param name="idProducto">El ID del producto cuyo stock se va a ajustar.</param>
        /// <param name="cantidadAjuste">La cantidad a añadir (positivo) o restar (negativo) al stock actual.
        /// Al finalizar, este parámetro contendrá el nuevo stock total.</param>
        /// <returns>El nuevo stock total del producto después del ajuste.</returns>
        public int ActualizarStockProductoViaSP(int idProducto, int cantidadAjuste)
        {
            string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            int nuevoStock = -1; // Valor por defecto en caso de error

            using (MySqlConnection conn = new MySqlConnection(currentConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("ActualizarStockProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Indica que es un procedimiento almacenado

                    // Parámetro IN: id_producto
                    cmd.Parameters.AddWithValue("p_id_producto", idProducto);

                    // Parámetro INOUT: p_cantidad_ajuste
                    MySqlParameter ajusteParam = new MySqlParameter("p_cantidad_ajuste", MySqlDbType.Int32);
                    ajusteParam.Direction = ParameterDirection.InputOutput; // Establece la dirección como INOUT
                    ajusteParam.Value = cantidadAjuste; // Valor inicial que se envía al SP (el ajuste)
                    cmd.Parameters.Add(ajusteParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado

                        // Recuperar el valor final del parámetro INOUT (el nuevo stock)
                        if (ajusteParam.Value != DBNull.Value)
                        {
                            nuevoStock = Convert.ToInt32(ajusteParam.Value);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Log.Escribir($"Error de base de datos al actualizar stock vía SP (INOUT) para producto ID {idProducto} con ajuste {cantidadAjuste}: {ex.Message}", ex);
                        throw new Exception("Error al actualizar el stock del producto desde la base de datos. Consulte los logs para más detalles.", ex);
                    }
                    catch (Exception ex)
                    {
                        Log.Escribir($"Error inesperado al actualizar stock vía SP (INOUT) para producto ID {idProducto} con ajuste {cantidadAjuste}: {ex.Message}", ex);
                        throw new Exception("Error inesperado al actualizar el stock del producto. Consulte los logs para más detalles.", ex);
                    }
                }
            }
            return nuevoStock;
        }


        // Implements the specific method from IProductoRepository to get a product by its exact name
        public producto GetByNombre(string nombre)
        {
            try
            {
                string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

                using (IDbConnection db = new MySqlConnection(currentConnectionString))
                {
                    string sql = "SELECT * FROM producto WHERE nombre = @Nombre LIMIT 1";
                    return db.QueryFirstOrDefault<producto>(sql, new { Nombre = nombre });
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener producto por nombre: {nombre}", ex);
                return null;
            }
        }

        // Implements the specific method from IProductoRepository to get products by a partial name match
        public List<producto> GetProductosByNombre(string nombreParcial)
        {
            List<producto> listaProductos = new List<producto>();
            try
            {
                string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

                using (IDbConnection db = new MySqlConnection(currentConnectionString))
                {
                    string sql = "SELECT * FROM producto WHERE nombre LIKE @NombreParcial";
                    listaProductos = db.Query<producto>(sql, new { NombreParcial = $"%{nombreParcial}%" }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener productos por nombre parcial: {nombreParcial}", ex);
                return new List<producto>();
            }
            return listaProductos;
        }

        public List<producto> GetProductosBySearchTerm(string searchTerm)
        {
            List<producto> listaProductos = new List<producto>();
            try
            {
                string currentConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

                using (IDbConnection db = new MySqlConnection(currentConnectionString))
                {
                    if (int.TryParse(searchTerm, out int idProducto))
                    {
                        string sql = "SELECT * FROM producto WHERE id_producto = @IdProducto";
                        listaProductos = db.Query<producto>(sql, new { IdProducto = idProducto }).ToList();
                    }
                    else
                    {
                        string sql = "SELECT * FROM producto WHERE nombre LIKE @SearchTerm OR descripcion LIKE @SearchTerm";
                        listaProductos = db.Query<producto>(sql, new { SearchTerm = $"%{searchTerm}%" }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener productos por término de búsqueda: {searchTerm}", ex);
                return new List<producto>();
            }
            return listaProductos;
        }
    }
}