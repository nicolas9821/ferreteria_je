// ferreteria_je\vistas\reportes.aspx.cs
using System;
using System.Web.UI.WebControls;
using ferreteria_je.session;

namespace ferreteria_je.vistas
{
    public partial class reportes : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No hay necesidad de cargar datos aquí
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnReporteCompras_Click(object sender, EventArgs e)
        {
            Response.Redirect("reportes_compras.aspx");
        }

        protected void btnReporteVentas_Click(object sender, EventArgs e)
        {
            Response.Redirect("reportes_ventas.aspx");
        }

        // Estos métodos ya no son necesarios porque los reportes se muestran dentro de las páginas generales
        // protected void btnReporteVentasProductos_Click(object sender, EventArgs e) { /* ... */ }
        // protected void btnReporteComprasProveedor_Click(object sender, EventArgs e) { /* ... */ }
    }
}