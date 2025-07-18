// ferreteria_je\vistas\reportes_ventas.aspx.cs
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using System.Globalization; // Para DateTime.TryParseExact
using ferreteria_je.session; // Asegúrate de tener esta referencia si usas BasePage

namespace ferreteria_je.vistas
{
    public partial class reportes_ventas : BasePage // Hereda de BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Establecer fechas por defecto al cargar por primera vez
                txtFechaInicio.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtFechaFin.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadAllVentasReports();
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            LoadAllVentasReports();
        }

        private void LoadAllVentasReports()
        {
            ReporteVentaRepository ventaRepo = new ReporteVentaRepository();
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            if (DateTime.TryParseExact(txtFechaInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFechaInicio))
            {
                fechaInicio = parsedFechaInicio;
            }

            if (DateTime.TryParseExact(txtFechaFin.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFechaFin))
            {
                fechaFin = parsedFechaFin.Date.AddDays(1).AddTicks(-1);
            }

            // --- Cargar Reporte de Detalle de Ventas ---
            try
            {
                var ventasDetalle = ventaRepo.GetReporteDetalle(fechaInicio, fechaFin).ToList();

                decimal totalGeneral = ventasDetalle.GroupBy(v => v.IdVenta)
                                                   .Sum(g => g.First().TotalVenta);

                gvVentas.DataSource = ventasDetalle;
                gvVentas.DataBind();
                lblTotalGeneralVentas.Text = $"Total General de Ventas: {totalGeneral:C}";
            }
            catch (Exception ex)
            {
                // Manejo de errores para el detalle de ventas
                // Response.Write($"Error al cargar el detalle de ventas: {ex.Message}");
                gvVentas.EmptyDataText = $"Error al cargar el detalle de ventas: {ex.Message}";
                gvVentas.DataSource = null; // Asegurar que no se muestren datos parciales
                gvVentas.DataBind();
                lblTotalGeneralVentas.Text = "Error al calcular el total.";
            }

            // --- Cargar Reporte de Ventas por Producto Agregado ---
            try
            {
                var ventasPorProducto = ventaRepo.GetReporteVentasPorProductoAgregado(fechaInicio, fechaFin).ToList();

                decimal totalGananciaEstimada = ventasPorProducto.Sum(p => p.GananciaEstimadaAgregado);

                gvVentasPorProducto.DataSource = ventasPorProducto;
                gvVentasPorProducto.DataBind();
                lblTotalGananciaEstimada.Text = $"Ganancia Estimada Total: {totalGananciaEstimada:C}";
            }
            catch (Exception ex)
            {
                // Manejo de errores para el reporte por producto
                // Response.Write($"Error al cargar el reporte de ventas por producto: {ex.Message}");
                gvVentasPorProducto.EmptyDataText = $"Error al cargar ventas por producto: {ex.Message}";
                gvVentasPorProducto.DataSource = null;
                gvVentasPorProducto.DataBind();
                lblTotalGananciaEstimada.Text = "Error al calcular la ganancia estimada.";
            }
        }
    }
}