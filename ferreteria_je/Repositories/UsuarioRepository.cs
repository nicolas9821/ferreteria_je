// ferreteria_je\Repositories\UsuarioRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models; // Asegura que apunta al modelo 'usuario'
using ferreteria_je.Repositories.RepositoriesGeneric;
using ferreteria_je.Utilidades;

namespace ferreteria_je.Repositories
{
    public class UsuarioRepository : GenericRepository<usuario>, IUsuarioRepository // <-- ¡AHORA ES 'usuario' (singular)!
    {
        public UsuarioRepository() : base() // Llama al constructor de la clase base
        {
        }

        public usuario GetByEmail(string email) // <-- ¡AHORA ES 'usuario' (singular) aquí!
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM usuarios WHERE email = @Email LIMIT 1"; // La tabla DB sigue siendo 'usuarios'
                    return db.QueryFirstOrDefault<usuario>(sql, new { Email = email }); // <-- ¡AHORA ES 'usuario' (singular)!
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener usuario por email: {email}", ex);
                return null;
            }
        }

        public List<usuario> GetUsuariosByNombre(string nombreParcial) // <-- ¡AHORA ES 'usuario' (singular) aquí!
        {
            List<usuario> listaUsuarios; // Eliminamos la inicialización redundante
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM usuarios WHERE nombre LIKE @NombreParcial"; // La tabla DB sigue siendo 'usuarios'
                    listaUsuarios = db.Query<usuario>(sql, new { NombreParcial = $"%{nombreParcial}%" }).ToList(); // <-- ¡AHORA ES 'usuario' (singular)!
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al obtener usuarios por nombre parcial: {nombreParcial}", ex);
                return new List<usuario>();
            }
            return listaUsuarios;
        }
    }
}