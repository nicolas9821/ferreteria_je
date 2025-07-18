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
    public partial class cajero_ventas : BasePage // Heredar de BasePage
    {
        // Declarar como IVentaRepository (interfaz)
        private IVentaRepository _ventaRepository;
        private IUsuarioRepository _usuarioRepository; // Necesario para obtener datos del usuario logeado

        protected void Page_Init(object sender, EventArgs e)
        {
            // Inicializar los repositorios
            _ventaRepository = new VentaRepository(); // Instanciar la implementación concreta
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

                CargarVentas();
                CargarDatosUsuarioEnInterfaz();
            }
        }

        private void CargarVentas()
        {
            try
            {
                string searchTerm = txtBuscarVenta.Text.Trim();
                List<venta> ventas;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    ventas = _ventaRepository.GetAll().ToList();
                }
                else
                {
                    // Esto ahora funcionará porque el método está en la interfaz
                    // Asume que GetVentasBySearchTerm filtra correctamente los datos.
                    ventas = _ventaRepository.GetVentasBySearchTerm(searchTerm);
                }

                if (ventas != null && ventas.Any())
                {
                    gvVentas.DataSource = ventas;
                    gvVentas.DataBind();
                    gvVentas.Visible = true;
                    lblNoResults.Visible = false;
                }
                else
                {
                    gvVentas.DataSource = null;
                    gvVentas.DataBind();
                    gvVentas.Visible = false;
                    lblNoResults.Text = "No se encontraron ventas.";
                    lblNoResults.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar ventas: {ex.Message}");
                lblNoResults.Text = "Ocurrió un error al cargar las ventas.";
                lblNoResults.Visible = true;
                gvVentas.Visible = false;
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

        protected void btnBuscarVenta_Click(object sender, EventArgs e)
        {
            CargarVentas();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExportarGridViewAExcel(gvVentas, "Ventas");
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

        protected void gvVentas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVentas.PageIndex = e.NewPageIndex;
            CargarVentas();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }
    }
}