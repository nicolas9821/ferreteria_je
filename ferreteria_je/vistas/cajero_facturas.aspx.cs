using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories; // Para tus repositorios
using ferreteria_je.Repositories.Models; // Para tus modelos
using ferreteria_je.Repositories.Interfaces; // Para tus interfaces de repositorio
using System.IO; // Para manejar archivos para la exportación
using System.Text; // Para codificación para la exportación
using ferreteria_je.session; // Asegúrate de que esta sea la ruta correcta a tu BasePage

namespace ferreteria_je
{
    public partial class cajero_facturas : BasePage // Heredar de BasePage
    {
        private IFacturaRepository _facturaRepository;
        private IUsuarioRepository _usuarioRepository; // Necesario para obtener datos del usuario logeado

        protected void Page_Init(object sender, EventArgs e)
        {
            _facturaRepository = new FacturaRepository();
            _usuarioRepository = new UsuarioRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ** Validar Rol del Cajero **
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "cajero")
                {
                    Response.Redirect("~/vistas/login.aspx"); // Redirigir si no es cajero
                    return; // Importante para detener la ejecución
                }

                CargarFacturas();
                CargarDatosUsuarioEnInterfaz();
            }
        }

        private void CargarFacturas()
        {
            try
            {
                string searchTerm = txtBuscarFactura.Text.Trim();
                List<factura> facturas;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    facturas = _facturaRepository.GetFacturasWithClientName(""); // Cargar todas si no hay término
                }
                else
                {
                    facturas = _facturaRepository.GetFacturasWithClientName(searchTerm); // Filtrar por término
                }

                if (facturas != null && facturas.Any())
                {
                    gvFacturas.DataSource = facturas;
                    gvFacturas.DataBind();
                    gvFacturas.Visible = true;
                    lblNoResults.Visible = false;
                }
                else
                {
                    gvFacturas.DataSource = null;
                    gvFacturas.DataBind();
                    gvFacturas.Visible = false;
                    lblNoResults.Text = "No se encontraron facturas.";
                    lblNoResults.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar facturas: {ex.Message}");
                lblNoResults.Text = "Ocurrió un error al cargar las facturas.";
                lblNoResults.Visible = true;
                gvFacturas.Visible = false;
            }
        }

        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserName.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        protected void btnBuscarFactura_Click(object sender, EventArgs e)
        {
            CargarFacturas();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExportarGridViewAExcel(gvFacturas, "Facturas");
        }

        private void ExportarGridViewAExcel(GridView gridView, string nombreArchivo)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo + ".xls");
            Response.Charset = "UTF-8"; // Asegurar la codificación para caracteres especiales
            Response.ContentType = "application/vnd.ms-excel";

            // Para evitar que los estilos CSS se exporten y causen problemas de formato en Excel
            gridView.EnableViewState = false; // Deshabilitar ViewState para no restaurar estilos después
            gridView.HeaderStyle.CssClass = "";
            gridView.RowStyle.CssClass = "";
            gridView.AlternatingRowStyle.CssClass = "";
            gridView.PagerStyle.CssClass = "";
            foreach (DataControlField column in gridView.Columns)
            {
                if (column is BoundField boundField)
                {
                    boundField.HeaderStyle.CssClass = "";
                    boundField.ItemStyle.CssClass = "";
                }
                else if (column is TemplateField templateField)
                {
                    templateField.HeaderStyle.CssClass = "";
                    templateField.ItemStyle.CssClass = "";
                }
            }

            // CREAR UN PANEL TEMPORAL PARA RENDERIZAR SOLO EL GRIDVIEW
            Panel p = new Panel();
            p.Controls.Add(gridView);

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    p.RenderControl(hw); // Renderizar el panel que contiene solo el GridView
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }

            // NOTA: No es necesario restaurar estilos aquí si EnableViewState=false,
            // porque la página se recargará completamente después de Response.End().
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Necesario para que el GridView pueda ser renderizado para la exportación a Excel
        }

        protected void gvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFacturas.PageIndex = e.NewPageIndex;
            CargarFacturas();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }
    }
}