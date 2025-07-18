// ferreteria_je.Repositories/ClienteRepository.cs
using MySql.Data.MySqlClient; // Para MySqlConnection
using System;
using System.Collections.Generic;
using System.Data; // Para IDbConnection
using System.Linq; // Para .ToList()
using Dapper; // Para usar el método Query<T> para consultas personalizadas
using ferreteria_je.Repositories.Interfaces; // Tu interfaz IClienteRepository
using ferreteria_je.Repositories.Models; // Tu modelo 'cliente'
using ferreteria_je.Repositories.RepositoriesGeneric; // Tu GenericRepository
using ferreteria_je.Utilidades; // Tu clase Log (asumo que la usas para manejar errores)

namespace ferreteria_je.Repositories
{
    // Hereda de GenericRepository<cliente> para los métodos CRUD genéricos
    // Implementa IClienteRepository para los métodos específicos de cliente
    public class ClienteRepository : GenericRepository<cliente>, IClienteRepository
    {
        // El constructor vacío está bien, ya que el 'connectionString'
        // se maneja en la clase base GenericRepository.
        public ClienteRepository()
        {
        }

        // --- Implementación de los métodos específicos de IClienteRepository ---

        public cliente GetByCedula(string cedula)
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    // Consulta SQL para buscar un cliente por su cédula
                    string sql = "SELECT * FROM cliente WHERE cedula = @Cedula LIMIT 1";
                    // Dapper.QueryFirstOrDefault<T> es ideal para obtener 0 o 1 resultado.
                    return db.QueryFirstOrDefault<cliente>(sql, new { Cedula = cedula });
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener cliente por cédula: {cedula}", ex);
                return null; // Retorna null si ocurre un error
            }
        }

        public List<cliente> GetClientesBySearchTerm(string searchTerm)
        {
            List<cliente> listaClientes = new List<cliente>();
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    // Consulta SQL para buscar clientes por nombre, cédula, teléfono o email
                    // Se usa LIKE y el operador '%' para búsquedas parciales.
                    string sql = "SELECT * FROM cliente " +
                                 "WHERE nombre LIKE @SearchTerm " +
                                 "OR cedula LIKE @SearchTerm " +
                                 "OR telefono LIKE @SearchTerm " +
                                 "OR email LIKE @SearchTerm";

                    // Asegúrate de envolver el parámetro con '%' en el valor, no en la consulta.
                    listaClientes = db.Query<cliente>(sql, new { SearchTerm = $"%{searchTerm}%" }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener clientes por término de búsqueda: {searchTerm}", ex);
                // Retorna una lista vacía si hay un error para evitar que la UI falle.
                return new List<cliente>();
            }
            return listaClientes;
        }
    }
}