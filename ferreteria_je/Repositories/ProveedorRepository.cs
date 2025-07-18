// ferreteria_je.Repositories/ProveedorRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq; // Necesario para .ToList()
using System.Configuration; // Necesario si usas connectionString directamente
using ferreteria_je.Repositories.Interfaces; // Tu interfaz IProveedorRepository
using ferreteria_je.Repositories.Models; // Tu modelo 'proveedor'
using ferreteria_je.Repositories.RepositoriesGeneric; // Tu GenericRepository
using ferreteria_je.Utilidades; // Tu clase Log
using Dapper; // Para consultas personalizadas (ej. QueryFirstOrDefault)

namespace ferreteria_je.Repositories
{
    // Hereda de GenericRepository<proveedor> para los métodos CRUD genéricos
    // Implementa IProveedorRepository para los métodos específicos
    public class ProveedorRepository : GenericRepository<proveedor>, IProveedorRepository
    {
        // El constructor vacío está bien. El 'connectionString' se hereda de GenericRepository.
        public ProveedorRepository()
        {
            // Puedes añadir lógica específica si la necesitas
        }

        // --- Métodos Específicos para Proveedor (similares a los de PacienteRepository) ---

        // Ejemplo: Obtener un proveedor por su correo electrónico
        public proveedor GetByEmail(string email)
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    // Consulta SQL para buscar un proveedor por email
                    string sql = "SELECT * FROM proveedor WHERE email = @Email LIMIT 1"; // LIMIT 1 si esperas solo uno
                    // Dapper.QueryFirstOrDefault<T> es ideal para obtener 0 o 1 resultado.
                    return db.QueryFirstOrDefault<proveedor>(sql, new { Email = email });
                }
            }
            catch (Exception ex)
            {
                // Usa tu clase Log para registrar el error
                Log.Escribir($"Error al obtener proveedor por email: {email}", ex);
                return null; // Retorna null si ocurre un error
            }
        }

        // Ejemplo: Obtener una lista de proveedores cuyo nombre contenga una cadena (filtro)
        public List<proveedor> GetProveedoresByNombre(string nombreParcial)
        {
            List<proveedor> listaProveedores = new List<proveedor>();
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM proveedor WHERE nombre LIKE @NombreParcial";
                    // Usa '%' para buscar coincidencias parciales.
                    // Asegúrate de envolver el parámetro con '%' en el valor, no en la consulta.
                    listaProveedores = db.Query<proveedor>(sql, new { NombreParcial = $"%{nombreParcial}%" }).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener proveedores por nombre parcial: {nombreParcial}", ex);
                // Retorna una lista vacía o relanza la excepción, según tu estrategia de manejo de errores.
                return new List<proveedor>();
            }
            return listaProveedores;
        }

        // Si necesitas métodos para obtener Proveedor y sus "Compras" o "Productos" relacionados
        // que no se mapean directamente en el modelo 'proveedor', ahí sí necesitarías un Dapper multi-mapping
        // similar a como lo tenías en PacienteRepository, pero solo si 'proveedor' tuviera propiedades
        // para esas colecciones y las quisieras cargar en el mismo objeto.
        // Dado tu esquema, es más probable que manejes las "Compras de un Proveedor"
        // desde un CompraRepository, filtrando por id_proveedor.
    }
}

