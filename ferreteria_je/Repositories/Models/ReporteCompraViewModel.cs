// ferreteria_je\Repositories\Models\ReporteCompraViewModel.cs
using System;
using System.Collections.Generic; // Necesario para List<T>
using System.Linq;
using System.Web;

namespace ferreteria_je.Repositories.Models
{
    public class ReporteCompraViewModel
    {
        // Propiedades existentes para el reporte de detalle de compras
        public int IdCompra { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NombreProveedor { get; set; }
        public string EmailProveedor { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadComprada { get; set; }
        public decimal PrecioUnitarioCompra { get; set; }
        public decimal SubtotalLinea { get; set; }
        public decimal TotalCompra { get; set; }

        // --- Nuevas propiedades para el reporte consolidado de Compras por Proveedor ---
        // Similar al de ventas, estas propiedades se usarán cuando el ViewModel represente
        // el reporte agregado por proveedor.
        public int IdProveedorAgregado { get; set; } // Usar un nombre diferente
        public string NombreProveedorAgregado { get; set; }
        public decimal TotalComprasAgregado { get; set; }
    }
}