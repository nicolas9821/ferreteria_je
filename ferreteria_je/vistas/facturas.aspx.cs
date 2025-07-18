using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System.IO; // Necesario para la exportación a Excel
using MySql.Data.MySqlClient; // Para manejo específico de errores MySQL (ej. foreign key)

namespace ferreteria_je
{
    public partial class facturas : ferreteria_je.session.BasePage
    {
        private FacturaRepository _facturaRepository;

        // Propiedades para la ordenación del GridView
        private string _sortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private string _sortExpression
        {
            get { return ViewState["SortExpression"] as string ?? ""; }
            set { ViewState["SortExpression"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _facturaRepository = new FacturaRepository();

            if (!IsPostBack)
            {
                // *** RECOMENDACIÓN: Validación de rol "admin" o "ventas" ***
                if (Session["UserRole"] == null || (Session["UserRole"].ToString().ToLower() != "admin" && Session["UserRole"].ToString().ToLower() != "ventas"))
                {
                    Response.Redirect("~/vistas/login.aspx"); // Redirigir si no tiene el rol adecuado
                    return; // Detener la ejecución de la página
                }

                CargarDatosUsuarioEnInterfaz(); // Cargar datos del usuario logeado en los literales
                CargarFacturas(); // Carga todas las facturas al inicio
            }
        }

        /// <summary>
        /// Carga los datos del usuario logeado en los literales de la interfaz (topbar).
        /// </summary>
        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser) // Asume que el objeto de sesión es de tipo 'usuario'
            {
                litUserNameButton.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                // Si no hay usuario en sesión, redirigir al login
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        /// <summary>
        /// Carga las facturas desde la base de datos en el GridView.
        /// Aplica filtrado y ordenación.
        /// </summary>
        private void CargarFacturas() // Ya no recibe argumento, obtiene el searchTerm de txtSearchFactura
        {
            List<factura> listaFacturas;
            try
            {
                string searchTerm = txtSearchFactura.Text.Trim(); // Obtiene el término de búsqueda aquí

                // Utiliza el método que carga el nombre del cliente y aplica el filtro
                listaFacturas = _facturaRepository.GetFacturasWithClientName(searchTerm);

                // Aplicar ordenación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_factura": listaFacturas = listaFacturas.OrderBy(f => f.id_factura).ToList(); break;
                            case "fecha": listaFacturas = listaFacturas.OrderBy(f => f.fecha).ToList(); break;
                            case "nombre_cliente": listaFacturas = listaFacturas.OrderBy(f => f.nombre_cliente).ToList(); break;
                            case "total": listaFacturas = listaFacturas.OrderBy(f => f.total).ToList(); break;
                            default: listaFacturas = listaFacturas.OrderBy(f => f.fecha).ToList(); break; // Orden por defecto
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_factura": listaFacturas = listaFacturas.OrderByDescending(f => f.id_factura).ToList(); break;
                            case "fecha": listaFacturas = listaFacturas.OrderByDescending(f => f.fecha).ToList(); break;
                            case "nombre_cliente": listaFacturas = listaFacturas.OrderByDescending(f => f.nombre_cliente).ToList(); break;
                            case "total": listaFacturas = listaFacturas.OrderByDescending(f => f.total).ToList(); break;
                            default: listaFacturas = listaFacturas.OrderByDescending(f => f.fecha).ToList(); break;
                        }
                    }
                }
                else
                {
                    // Orden por defecto si no hay expresión de ordenación
                    listaFacturas = listaFacturas.OrderByDescending(f => f.fecha).ToList(); // Generalmente las facturas se muestran de la más reciente a la más antigua
                }

                gvFacturas.DataSource = listaFacturas;
                gvFacturas.DataBind();

                if (listaFacturas.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
                {
                    SetMessage("No se encontraron facturas con el término de búsqueda.", "info");
                }
                else if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    ClearMessages(); // Limpiar mensajes si no hay búsqueda
                }
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar facturas en la página: " + ex.Message, ex);
                SetMessage("Error al cargar las facturas. Consulte los logs para más detalles.", "error");
            }
        }

        /// <summary>
        /// Muestra un mensaje al usuario.
        /// </summary>
        private void SetMessage(string message, string type)
        {
            // Oculta ambos mensajes primero
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert hidden";
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden";

            // Muestra el mensaje apropiado
            if (type == "success" || type == "info")
            {
                lblMessage.Text = message;
                lblMessage.CssClass = $"alert alert-{type} visible"; // e.g., "alert alert-success visible"
            }
            else if (type == "error")
            {
                lblErrorMessage.Text = message;
                lblErrorMessage.CssClass = "alert alert-danger visible";
            }
        }

        /// <summary>
        /// Limpia los mensajes mostrados al usuario.
        /// </summary>
        private void ClearMessages()
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert hidden";
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden";
        }

        protected void gvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFacturas.PageIndex = e.NewPageIndex;
            CargarFacturas(); // SIN ARGUMENTOS
        }

        protected void gvFacturas_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (_sortExpression == e.SortExpression)
            {
                _sortDirection = (_sortDirection == "ASC" ? "DESC" : "ASC");
            }
            else
            {
                _sortExpression = e.SortExpression;
                _sortDirection = "ASC"; // Por defecto, si cambia la columna, se ordena ascendente
            }
            CargarFacturas(); // SIN ARGUMENTOS
        }

        protected void gvFacturas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idFactura = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarFactura")
            {
                Response.Redirect($"gestionfacturas.aspx?id={idFactura}");
            }
            else if (e.CommandName == "EliminarFactura")
            {
                EliminarFactura(idFactura);
                CargarFacturas(); // SIN ARGUMENTOS
            }
        }

        private void EliminarFactura(int idFactura)
        {
            try
            {
                // Al eliminar una factura, se debe eliminar también sus detalles asociados
                // Esto puede hacerse en cascada en la DB, o manejarse explícitamente en el repositorio.
                // Aquí se asume que el repositorio maneja la eliminación de detalles o que la DB tiene CASCADE DELETE.
                var facturaAEliminar = new factura { id_factura = idFactura };
                _facturaRepository.Delete(facturaAEliminar);
                SetMessage("Factura eliminada exitosamente junto con sus detalles.", "success");
            }
            catch (MySqlException ex)
            {
                // Código de error 1451 es para violación de clave foránea en MySQL (si no tienes CASCADE DELETE)
                if (ex.Number == 1451)
                {
                    SetMessage("No se puede eliminar la factura porque tiene registros asociados (ej. detalles de factura). Asegúrese de que no haya detalles vinculados o que la base de datos maneje la eliminación en cascada.", "error");
                }
                else
                {
                    Log.Escribir($"Error de BD al eliminar factura con ID {idFactura}: {ex.Message}", ex);
                    SetMessage("Error de base de datos al eliminar la factura. Intente de nuevo o contacte soporte.", "error");
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar factura con ID {idFactura}: {ex.Message}", ex);
                SetMessage($"Error al eliminar la factura: {ex.Message}. Intente de nuevo más tarde.", "error");
            }
        }

        protected void lnkAgregarFactura_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionfacturas.aspx");
        }

        protected void btnSearchFactura_Click(object sender, EventArgs e)
        {
            gvFacturas.PageIndex = 0; // Reiniciar página al buscar
            CargarFacturas(); // SIN ARGUMENTOS
        }

        protected void txtSearchFactura_TextChanged(object sender, EventArgs e)
        {
            gvFacturas.PageIndex = 0; // Reiniciar página al cambiar texto y autopostback
            CargarFacturas(); // SIN ARGUMENTOS
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        // ****** FUNCIONALIDAD DE EXPORTAR A EXCEL ******
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearchFactura.Text.Trim();
                List<factura> facturasToExport;

                facturasToExport = _facturaRepository.GetFacturasWithClientName(searchTerm);

                // Aplicar ordenación a los datos a exportar también
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_factura": facturasToExport = facturasToExport.OrderBy(f => f.id_factura).ToList(); break;
                            case "fecha": facturasToExport = facturasToExport.OrderBy(f => f.fecha).ToList(); break;
                            case "nombre_cliente": facturasToExport = facturasToExport.OrderBy(f => f.nombre_cliente).ToList(); break;
                            case "total": facturasToExport = facturasToExport.OrderBy(f => f.total).ToList(); break;
                            default: facturasToExport = facturasToExport.OrderByDescending(f => f.fecha).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "id_factura": facturasToExport = facturasToExport.OrderByDescending(f => f.id_factura).ToList(); break;
                            case "fecha": facturasToExport = facturasToExport.OrderByDescending(f => f.fecha).ToList(); break;
                            case "nombre_cliente": facturasToExport = facturasToExport.OrderByDescending(f => f.nombre_cliente).ToList(); break;
                            case "total": facturasToExport = facturasToExport.OrderByDescending(f => f.total).ToList(); break;
                            default: facturasToExport = facturasToExport.OrderByDescending(f => f.fecha).ToList(); break;
                        }
                    }
                }
                else
                {
                    facturasToExport = facturasToExport.OrderByDescending(f => f.fecha).ToList();
                }

                // Crear un GridView temporal para la exportación
                GridView gvExport = new GridView();
                // Selecciona solo las propiedades que quieres exportar
                gvExport.DataSource = facturasToExport.Select(f => new { f.id_factura, f.fecha, f.nombre_cliente, f.total }).ToList();
                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Facturas_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        gvExport.RenderControl(hw);
                        Response.Write(sw.ToString());
                    }
                }

                Response.Flush();
                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Esta excepción es normal y esperada cuando se usa Response.End()
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar facturas a Excel: {ex.Message}", ex);
                SetMessage($"Ocurrió un error al exportar los datos: {ex.Message}", "error");
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}