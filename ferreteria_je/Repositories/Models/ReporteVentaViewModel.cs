// ferreteria_je\Repositories\Models\ReporteVentaViewModel.cs
using System;
using System.Collections.Generic; // Necesario para List<T>
using System.Linq;
using System.Web;

namespace ferreteria_je.Repositories.Models
{
    public class ReporteVentaViewModel
    {
        // Propiedades existentes para el reporte de detalle de ventas
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; } // Opcional
        public string NombreUsuario { get; set; } // Vendedor/Cajero
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public decimal PrecioUnitarioVenta { get; set; }
        public decimal SubtotalLinea { get; set; } // Cantidad * PrecioUnitario
        public decimal TotalVenta { get; set; } // El total de la tabla 'venta' para esa venta

        // --- Nuevas propiedades para el reporte consolidado de Ventas por Producto ---
        // Nota: Estas propiedades SOLO se usarán cuando el ViewModel sea parte de una lista
        // que representa el reporte agregado por producto, no por cada línea de detalle de venta.
        // Es una forma de "reutilizar" el ViewModel para diferentes vistas del mismo reporte.
        public int IdProductoAgregado { get; set; } // Usar un nombre diferente para evitar conflictos si se mezcla
        public string NombreProductoAgregado { get; set; }
        public int CantidadTotalVendidaAgregado { get; set; }
        public decimal IngresoTotalGeneradoAgregado { get; set; }
        public decimal CostoTotalEstimadoAgregado { get; set; }
        public decimal GananciaEstimadaAgregado { get; set; }
    }
}