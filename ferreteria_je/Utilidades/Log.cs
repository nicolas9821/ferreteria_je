using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ferreteria_je.Utilidades
{
    public static class Log
    {
        private static readonly string logFileName = "errores.log";

        public static void Escribir(string mensaje, Exception ex = null)
        {
            try
            {
                string rutaLog = HttpContext.Current.Server.MapPath("~/Logs/" + logFileName);

                // Crea la carpeta si no existe
                string carpeta = Path.GetDirectoryName(rutaLog);
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string textoLog = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}";

                if (ex != null)
                {
                    textoLog += $"\nEXCEPCIÓN: {ex.Message}\nSTACKTRACE:\n{ex.StackTrace}";
                }

                textoLog += "\n-----------------------------\n";

                File.AppendAllText(rutaLog, textoLog);
            }
            catch
            {
                // Evitar que falle si el log falla
            }
        }
    }
}



