using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models; // Necesario para tu clase Usuario y compra
using ferreteria_je.Utilidades;
using System.IO; // Necesario para StringWriter y HtmlTextWriter

namespace ferreteria_je
{
    public partial class compras : ferreteria_je.session.BasePage
    {
        private CompraRepository _compraRepository;

        // Declaración de controles para el designer.cs y uso en el code-behind
        protected LinkButton lnkCerrarSesion;
        protected Literal litUserNameButton;
        protected Literal litUserFullName;
        protected Literal litUserEmail;
        protected Literal litUserPhone;
        protected LinkButton lnkNuevaCompra;
        protected LinkButton btnExportExcel;
        protected TextBox txtBuscar;
        protected LinkButton btnBuscar;
        protected Label lblMessage;
        protected Label lblErrorMessage;
        protected GridView gvCompras;

        protected void Page_Load(object sender, EventArgs e)
        {
            _compraRepository = new CompraRepository();

            if (!IsPostBack)
            {
                // *** RECOMENDACIÓN: Validación de rol "admin" aquí también si es necesario ***
                // Similar a como lo tienes en clientes.aspx.cs, podrías añadir:
                // if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
                // {
                //     Response.Redirect("~/vistas/login.aspx");
                //     return;
                // }

                CargarDatosUsuarioTopbar(); // Llamamos a esta función al cargar la página
                CargarCompras();
            }
            lblMessage.Visible = false;
            lblErrorMessage.Visible = false;
        }

        private void CargarDatosUsuarioTopbar()
        {
            // ***** AJUSTE CLAVE AQUÍ: Basado en tu archivo de referencia 'clientes.aspx.cs' *****
            // Intentamos obtener el objeto 'usuario' (con 'u' minúscula) completo de la sesión.
            if (Session["usuario"] is usuario currentUser) // Usamos 'is' para un patrón de coincidencia y asignación
            {
                // Mostramos el nombre del usuario en el botón del topbar
                litUserNameButton.Text = currentUser.nombre;

                // Llenamos los literales del dropdown con los datos del usuario
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                // Si no hay usuario logeado en sesión o no es del tipo esperado
                litUserNameButton.Text = "Invitado";
                litUserFullName.Text = "N/A";
                litUserEmail.Text = "N/A";
                litUserPhone.Text = "N/A";

                // Opcional: Redirigir al login si no hay sesión (similar a clientes.aspx.cs)
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        private void CargarCompras(string searchTerm = "")
        {
            List<compra> listaCompras = new List<compra>();
            try
            {
                // ***** AJUSTE CLAVE AQUÍ: Se cambió a GetComprasWithProveedorName *****
                // Esto asume que este método SÍ existe en tu CompraRepository
                // y que devuelve una lista de objetos 'compra' que al menos tienen 'nombre_proveedor' y 'nombre_usuario'.
                // Si este método no existe o no devuelve 'nombre_proveedor' o 'nombre_usuario', los errores persistirán.
                listaCompras = _compraRepository.GetComprasWithProveedorName(searchTerm).ToList();

                gvCompras.DataSource = listaCompras;
                gvCompras.DataBind();

                if (listaCompras.Count == 0 && string.IsNullOrEmpty(searchTerm))
                {
                    lblMessage.Text = "No hay compras registradas en el sistema.";
                    lblMessage.CssClass = "alert alert-info visible";
                    lblMessage.Visible = true;
                }
                else if (listaCompras.Count == 0 && !string.IsNullOrEmpty(searchTerm))
                {
                    lblMessage.Text = "No se encontraron compras para la búsqueda: '" + Server.HtmlEncode(searchTerm) + "'";
                    lblMessage.CssClass = "alert alert-info visible";
                    lblMessage.Visible = true;
                }
                else
                {
                    lblMessage.Visible = false;
                    lblErrorMessage.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar compras en la página: " + ex.Message, ex);
                lblErrorMessage.Text = "Error al cargar compras: " + ex.Message;
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblErrorMessage.Visible = true;
            }
        }

        protected void gvCompras_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompras.PageIndex = e.NewPageIndex;
            CargarCompras(txtBuscar.Text.Trim());
        }

        protected void gvCompras_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idCompra = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarCompra")
            {
                Response.Redirect($"gestioncompras.aspx?id={idCompra}");
            }
            else if (e.CommandName == "EliminarCompra")
            {
                EliminarCompra(idCompra);
                CargarCompras(txtBuscar.Text.Trim());
            }
        }

        private void EliminarCompra(int idCompra)
        {
            try
            {
                _compraRepository.Delete(new compra { id_compra = idCompra });
                lblMessage.Text = "Compra eliminada exitosamente.";
                lblMessage.CssClass = "alert alert-success visible";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar compra con ID {idCompra}: {ex.Message}", ex);
                lblErrorMessage.Text = $"Error al eliminar compra: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblErrorMessage.Visible = true;
            }
        }

        protected void lnkNuevaCompra_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestioncompras.aspx");
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvCompras.PageIndex = 0;
            CargarCompras(txtBuscar.Text.Trim());
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            gvCompras.PageIndex = 0;
            CargarCompras(txtBuscar.Text.Trim());
        }

        protected void gvCompras_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;
            string sortDirection = GetSortDirection(sortExpression);

            // ***** AJUSTE CLAVE AQUÍ: Se cambió a GetComprasWithProveedorName *****
            List<compra> currentData = _compraRepository.GetComprasWithProveedorName(txtBuscar.Text.Trim()).ToList();

            // Ordenar los datos
            switch (sortExpression)
            {
                case "id_compra":
                    currentData = sortDirection == "ASC" ? currentData.OrderBy(c => c.id_compra).ToList() : currentData.OrderByDescending(c => c.id_compra).ToList();
                    break;
                case "fecha":
                    currentData = sortDirection == "ASC" ? currentData.OrderBy(c => c.fecha).ToList() : currentData.OrderByDescending(c => c.fecha).ToList();
                    break;
                case "nombre_proveedor":
                    // Asegúrate de que 'nombre_proveedor' exista en tu clase 'compra' y sea llenado por GetComprasWithProveedorName
                    currentData = sortDirection == "ASC" ? currentData.OrderBy(c => c.nombre_proveedor).ToList() : currentData.OrderByDescending(c => c.nombre_proveedor).ToList();
                    break;
                case "nombre_usuario":
                    // Si 'nombre_usuario' no es devuelto por GetComprasWithProveedorName, esta ordenación no funcionará como esperas
                    currentData = sortDirection == "ASC" ? currentData.OrderBy(c => c.nombre_usuario).ToList() : currentData.OrderByDescending(c => c.nombre_usuario).ToList();
                    break;
                case "total":
                    currentData = sortDirection == "ASC" ? currentData.OrderBy(c => c.total).ToList() : currentData.OrderByDescending(c => c.total).ToList();
                    break;
            }

            gvCompras.DataSource = currentData;
            gvCompras.DataBind();

            ViewState["SortExpression"] = sortExpression;
            ViewState["SortDirection"] = (ViewState["SortDirection"] != null && ViewState["SortExpression"].ToString() == sortExpression && ViewState["SortDirection"].ToString() == "ASC") ? "DESC" : "ASC";
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == column)
            {
                string lastDirection = ViewState["SortDirection"] as string;
                if (lastDirection == "ASC")
                {
                    sortDirection = "DESC";
                }
            }
            return sortDirection;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // ***** AJUSTE CLAVE AQUÍ: Se cambió a GetComprasWithProveedorName *****
                List<compra> allCompras = _compraRepository.GetComprasWithProveedorName("").ToList();

                if (allCompras.Count == 0)
                {
                    lblErrorMessage.Text = "No hay datos de compras para exportar a Excel.";
                    lblErrorMessage.CssClass = "alert alert-warning visible";
                    lblErrorMessage.Visible = true;
                    return;
                }

                GridView gvExport = new GridView();
                gvExport.AutoGenerateColumns = false;

                gvExport.Columns.Add(new BoundField { DataField = "id_compra", HeaderText = "ID Compra" });
                gvExport.Columns.Add(new BoundField { DataField = "fecha", HeaderText = "Fecha", DataFormatString = "{0:yyyy-MM-dd}" });
                gvExport.Columns.Add(new BoundField { DataField = "nombre_proveedor", HeaderText = "Proveedor" });
                // Si 'nombre_usuario' no es devuelto por GetComprasWithProveedorName, esta columna estará vacía
                gvExport.Columns.Add(new BoundField { DataField = "nombre_usuario", HeaderText = "Registrado por" });
                gvExport.Columns.Add(new BoundField { DataField = "total", HeaderText = "Total", DataFormatString = "{0:C}" });

                gvExport.DataSource = allCompras;
                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Compras.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        gvExport.RenderControl(hw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar compras a Excel: {ex.Message}", ex);
                lblErrorMessage.Text = $"Error al exportar a Excel: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblErrorMessage.Visible = true;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Este método es necesario cuando se renderiza un control de servidor como un GridView
             * fuera de su contexto de formulario ASP.NET normal, como al exportar a Excel.
             * Se anula para evitar el error "Control 'GridView' of type 'GridView' must be placed inside a form tag with runat=server."
             */
        }
    }
}