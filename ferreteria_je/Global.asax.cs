// Global.asax.cs
// Este archivo contiene la lógica de inicio global de tu aplicación ASP.NET.

using System;
using System.Web;
using System.Web.Optimization; // Necesario para BundleConfig
using System.Web.Routing; // Necesario para RouteConfig y RouteTable
using System.Reflection; // Necesario para GetCustomAttribute
using Dapper.Contrib.Extensions; // Necesario para SqlMapperExtensions y TableAttribute

namespace ferreteria_je
{
    public class Global : HttpApplication
    {
        /// <summary>
        /// Se ejecuta cuando la aplicación se inicia.
        /// Aquí se configuran los ajustes globales de la aplicación,
        /// como el mapeo de nombres de tabla para Dapper.Contrib.
        /// </summary>
        /// <param name="sender">El origen del evento.</param>
        /// <param name="e">Un objeto que no contiene datos de evento.</param>
        void Application_Start(object sender, EventArgs e)
        {
            // Configuración de rutas y bundles (mantén estas líneas si las usas)
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configura el TableNameMapper para Dapper.Contrib.
            // Esta configuración es CRUCIAL para que Dapper.Contrib interprete correctamente
            // el atributo [Table("usuarios")] que tienes en tu modelo 'usuario'.
            //
            // Lógica:
            // 1. Intenta obtener el 'TableAttribute' (ej. [Table("usuarios")]) de la clase.
            // 2. Si el atributo existe, usa el nombre especificado en él (ej. "usuarios").
            // 3. Si el atributo NO existe, usa el nombre de la clase como nombre de tabla
            //    (esto es un fallback para otras clases que no tengan el atributo [Table]).
            SqlMapperExtensions.TableNameMapper = (type) =>
            {
                var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                return tableAttribute != null ? tableAttribute.Name : type.Name;
            };

            // Si tienes otras configuraciones de inicio de la aplicación,
            // puedes agregarlas aquí.
        }

        // Puedes tener otros manejadores de eventos aquí, como:
        // void Session_Start(object sender, EventArgs e) { /* ... */ }
        // void Application_End(object sender, EventArgs e) { /* ... */ }
        // void Application_Error(object sender, EventArgs e) { /* ... */ }
        // void Session_End(object sender, EventArgs e) { /* ... */ }
        // void Application_BeginRequest(object sender, EventArgs e) { /* ... */ }
        // void Application_EndRequest(object sender, EventArgs e) { /* ... */ }
    }
}
