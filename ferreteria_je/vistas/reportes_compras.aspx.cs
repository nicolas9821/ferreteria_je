// ferreteria_je\vistas\reportes_compras.aspx.cs
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
    public partial class reportes_compras : BasePage // Hereda de BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaInicio.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtFechaFin.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadAllComprasReports();
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            LoadAllComprasReports();
        }

        private void LoadAllComprasReports()
        {
            ReporteCompraRepository compraRepo = new ReporteCompraRepository();
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

            // --- Cargar Reporte de Detalle de Compras ---
            try
            {
                var comprasDetalle = compraRepo.GetReporteDetalle(fechaInicio, fechaFin).ToList();

                decimal totalGeneral = comprasDetalle.GroupBy(c => c.IdCompra)
                                                     .Sum(g => g.First().TotalCompra);

                gvCompras.DataSource = comprasDetalle;
                gvCompras.DataBind();
                lblTotalGeneralCompras.Text = $"Total General de Compras: {totalGeneral:C}";
            }
            catch (Exception ex)
            {
                // Manejo de errores para el detalle de compras
                // Response.Write($"Error al cargar el detalle de compras: {ex.Message}");
                gvCompras.EmptyDataText = $"Error al cargar el detalle de compras: {ex.Message}";
                gvCompras.DataSource = null;
                gvCompras.DataBind();
                lblTotalGeneralCompras.Text = "Error al calcular el total.";
            }


            // --- Cargar Reporte de Compras por Proveedor Agregado ---
            try
            {
                var comprasPorProveedor = compraRepo.GetReporteComprasPorProveedorAgregado(fechaInicio, fechaFin).ToList();

                decimal totalGeneralComprasProveedor = comprasPorProveedor.Sum(p => p.TotalComprasAgregado);

                gvComprasPorProveedor.DataSource = comprasPorProveedor;
                gvComprasPorProveedor.DataBind();
                lblTotalGeneralComprasProveedor.Text = $"Total General Comprado a Proveedores: {totalGeneralComprasProveedor:C}";
            }
            catch (Exception ex)
            {
                // Manejo de errores para el reporte por proveedor
                // Response.Write($"Error al cargar el reporte de compras por proveedor: {ex.Message}");
                gvComprasPorProveedor.EmptyDataText = $"Error al cargar compras por proveedor: {ex.Message}";
                gvComprasPorProveedor.DataSource = null;
                gvComprasPorProveedor.DataBind();
                lblTotalGeneralComprasProveedor.Text = "Error al calcular el total de proveedores.";
            }
        }
    }
}