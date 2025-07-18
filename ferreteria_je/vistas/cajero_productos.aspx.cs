using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.Interfaces;
using System.IO;
using System.Text;
using ferreteria_je.session;

namespace ferreteria_je
{
    public partial class cajero_productos : BasePage
    {
        private IProductoRepository _productoRepository;
        private IUsuarioRepository _usuarioRepository;

        protected void Page_Init(object sender, EventArgs e)
        {
            _productoRepository = new ProductoRepository();
            _usuarioRepository = new UsuarioRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "cajero")
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarProductos();
                CargarDatosUsuarioEnInterfaz();
            }
        }

        private void CargarProductos()
        {
            try
            {
                string searchTerm = txtBuscarProducto.Text.Trim();
                List<producto> productos;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    productos = _productoRepository.GetAll().ToList();
                }
                else
                {
                    productos = _productoRepository.GetProductosBySearchTerm(searchTerm);
                }

                if (productos != null && productos.Any())
                {
                    gvProductos.DataSource = productos;
                    gvProductos.DataBind();
                    gvProductos.Visible = true;
                    lblNoResults.Visible = false;
                }
                else
                {
                    gvProductos.DataSource = null;
                    gvProductos.DataBind();
                    gvProductos.Visible = false;
                    lblNoResults.Text = "No se encontraron productos.";
                    lblNoResults.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar productos: {ex.Message}");
                lblNoResults.Text = "Ocurrió un error al cargar los productos.";
                lblNoResults.Visible = true;
                gvProductos.Visible = false;
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

        protected void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            ExportarGridViewAExcel(gvProductos, "Productos");
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

        // MUY IMPORTANTE: Asegúrate de que este método esté en la clase de tu página.
        // Si tu BasePage ya lo tiene, asegúrate de que no haya conflicto o duplicidad.
        public override void VerifyRenderingInServerForm(Control control)
        {
            /*
             * Confirma que un control de servidor ASP.NET está representado como contenido HTML
             * y que está correctamente contenido en un formulario <form runat="server">.
             *
             * Es necesario sobrescribirlo cuando se llama a RenderControl() en un control
             * que no está directamente dentro de la jerarquía de un formulario de servidor,
             * como cuando se exporta un GridView.
             */
        }

        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            CargarProductos();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }
    }
}