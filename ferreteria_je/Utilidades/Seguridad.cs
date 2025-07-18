// ferreteria_je\Utilidades\Seguridad.cs
using System;
using System.Security.Cryptography; // Necesario para SHA256
using System.Text; // Necesario para Encoding

namespace ferreteria_je.Utilidades // <-- Mismo namespace que tu clase Log
{
    public static class Seguridad // <-- Nueva clase estática para funciones de seguridad
    {
        /// <summary>
        /// Hashes una contraseña usando SHA256.
        /// ADVERTENCIA: Esta es una implementación básica y NO es segura para entornos de producción.
        /// Para producción, usa librerías como BCrypt.Net-Next o Microsoft.AspNetCore.Identity.PasswordHasher.
        /// </summary>
        /// <param name="password">La contraseña en texto plano.</param>
        /// <returns>La contraseña hasheada.</returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Aquí podrías añadir otros métodos relacionados con seguridad, como verificación de password, etc.
    }
}