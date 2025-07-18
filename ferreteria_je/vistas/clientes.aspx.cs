using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using ferreteria_je.session;
using MySql.Data.MySqlClient; // Para manejo específico de errores MySQL (ej. foreign key)

namespace ferreteria_je
{
    public partial class clientes : ferreteria_je.session.BasePage
    {
        private ClienteRepository _clienteRepository;

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
            _clienteRepository = new ClienteRepository();

            if (!IsPostBack)
            {
                // *** RECOMENDACIÓN: Validación de rol "admin" o "ventas" ***
                if (Session["UserRole"] == null || (Session["UserRole"].ToString().ToLower() != "admin" && Session["UserRole"].ToString().ToLower() != "ventas"))
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarDatosUsuarioEnInterfaz();
                CargarClientes(); // SIN ARGUMENTOS
                ClearMessages(); // ¡¡¡NUEVO!!! Limpiar mensajes en la carga inicial
            }
        }

        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserNameButton.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        /// <summary>
        /// Carga los clientes desde la base de datos en el GridView.
        /// Aplica filtrado y ordenación.
        /// </summary>
        private void CargarClientes() // YA NO RECIBE ARGUMENTOS
        {
            List<cliente> listaClientes;
            try
            {
                string searchTerm = txtBuscar.Text.Trim(); // Obtiene el término de búsqueda aquí

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    listaClientes = _clienteRepository.GetClientesBySearchTerm(searchTerm);
                }
                else
                {
                    listaClientes = _clienteRepository.GetAll().ToList();
                }

                // Aplicar ordenación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_cliente": listaClientes = listaClientes.OrderBy(c => c.id_cliente).ToList(); break;
                            case "cedula": listaClientes = listaClientes.OrderBy(c => c.cedula).ToList(); break;
                            case "nombre": listaClientes = listaClientes.OrderBy(c => c.nombre).ToList(); break;
                            case "direccion": listaClientes = listaClientes.OrderBy(c => c.direccion).ToList(); break;
                            case "telefono": listaClientes = listaClientes.OrderBy(c => c.telefono).ToList(); break;
                            case "email": listaClientes = listaClientes.OrderBy(c => c.email).ToList(); break;
                            default: listaClientes = listaClientes.OrderBy(c => c.nombre).ToList(); break;
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_cliente": listaClientes = listaClientes.OrderByDescending(c => c.id_cliente).ToList(); break;
                            case "cedula": listaClientes = listaClientes.OrderByDescending(c => c.cedula).ToList(); break;
                            case "nombre": listaClientes = listaClientes.OrderByDescending(c => c.nombre).ToList(); break;
                            case "direccion": listaClientes = listaClientes.OrderByDescending(c => c.direccion).ToList(); break;
                            case "telefono": listaClientes = listaClientes.OrderByDescending(c => c.telefono).ToList(); break;
                            case "email": listaClientes = listaClientes.OrderByDescending(c => c.email).ToList(); break;
                            default: listaClientes = listaClientes.OrderByDescending(c => c.nombre).ToList(); break;
                        }
                    }
                }
                else
                {
                    listaClientes = listaClientes.OrderBy(c => c.nombre).ToList();
                }

                gvClientes.DataSource = listaClientes;
                gvClientes.DataBind();

                // ¡¡¡AJUSTE CLAVE AQUÍ: Solo actualiza los mensajes de éxito si NO hay un mensaje de error activo!!!
                // Si lblErrorMessage.Text tiene contenido, significa que EliminarCliente ya puso un error,
                // y CargarClientes no debe sobrescribirlo con el mensaje de éxito.
                if (string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    if (listaClientes.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
                    {
                        SetMessage("No se encontraron clientes con el término de búsqueda.", "info");
                    }
                    else if (listaClientes.Count > 0)
                    {
                        SetMessage($"Se han cargado {listaClientes.Count()} clientes.", "info");
                    }
                    else // Si no hay clientes y no hay término de búsqueda (tabla vacía)
                    {
                        SetMessage("No se encontraron clientes.", "info");
                    }
                }
                // Si lblErrorMessage.Text NO está vacío, no hacemos nada aquí para preservar el mensaje de error.
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar clientes en la página: " + ex.Message, ex);
                SetMessage("Error al cargar los clientes. Consulte los logs para más detalles.", "error");
            }
        }

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
                lblMessage.CssClass = $"alert alert-{type} visible";
            }
            else if (type == "error")
            {
                lblErrorMessage.Text = message;
                lblErrorMessage.CssClass = "alert alert-danger visible";
            }
        }

        private void ClearMessages()
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert hidden";
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden";
        }

        protected void gvClientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvClientes.PageIndex = e.NewPageIndex;
            CargarClientes(); // SIN ARGUMENTOS
        }

        protected void gvClientes_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (_sortExpression == e.SortExpression)
            {
                _sortDirection = (_sortDirection == "ASC" ? "DESC" : "ASC");
            }
            else
            {
                _sortExpression = e.SortExpression;
                _sortDirection = "ASC";
            }
            CargarClientes(); // SIN ARGUMENTOS
        }

        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int clienteId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarCliente")
            {
                Response.Redirect($"gestionclientes.aspx?id={clienteId}");
            }
            else if (e.CommandName == "EliminarCliente")
            {
                EliminarCliente(clienteId);
                CargarClientes(); // SIN ARGUMENTOS
            }
        }

        private void EliminarCliente(int clienteId)
        {
            ClearMessages(); // ¡¡¡NUEVO!!! Limpia los mensajes antes de intentar eliminar

            try
            {
                var clienteAEliminar = new cliente { id_cliente = clienteId };
                _clienteRepository.Delete(clienteAEliminar);
                SetMessage("Cliente eliminado exitosamente.", "success");
            }
            catch (Exception ex) // Captura general para cualquier excepción
            {
                Log.Escribir($"Error al eliminar cliente con ID {clienteId}: {ex.Message}", ex);

                string userFriendlyErrorMessage = "Error al eliminar el cliente. Intente de nuevo más tarde.";

                // Itera a través de las excepciones internas para encontrar la causa raíz
                Exception currentEx = ex;
                while (currentEx != null)
                {
                    // ¡¡¡CAMBIO CLAVE AQUÍ: BUSCAR MySqlException!!!
                    if (currentEx is MySqlException mySqlEx)
                    {
                        // El código de error 1451 es para violación de clave foránea en MySQL
                        if (mySqlEx.Number == 1451)
                        {
                            userFriendlyErrorMessage = $"No se puede eliminar el cliente con ID {clienteId} porque tiene registros asociados (ej. ventas, factura). Elimine los registros asociados primero.";
                            break; // Error específico encontrado, salir del bucle
                        }
                    }
                    // Si usas un ORM (como Entity Framework) con MySQL, la MySqlException podría estar anidada
                    // Por ejemplo, si la excepción inicial es DbUpdateException y dentro tiene la MySqlException:
                    // if (currentEx is System.Data.Entity.Infrastructure.DbUpdateException dbUpdateEx)
                    // {
                    //     if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1451)
                    //     {
                    //         userFriendlyErrorMessage = $"No se puede eliminar el cliente con ID {clienteId} porque tiene registros asociados (ej. ventas). Elimine los registros asociados primero.";
                    //         break;
                    //     }
                    // }

                    currentEx = currentEx.InnerException;
                }

                SetMessage(userFriendlyErrorMessage, "error"); // Usa SetMessage para mostrar el error
            }
        }

        protected void lnkAgregarCliente_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionclientes.aspx");
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvClientes.PageIndex = 0;
            CargarClientes(); // SIN ARGUMENTOS
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            gvClientes.PageIndex = 0;
            CargarClientes(); // SIN ARGUMENTOS
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtBuscar.Text.Trim();
                List<cliente> clientesToExport;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    clientesToExport = _clienteRepository.GetClientesBySearchTerm(searchTerm);
                }
                else
                {
                    clientesToExport = _clienteRepository.GetAll().ToList();
                }

                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_cliente": clientesToExport = clientesToExport.OrderBy(c => c.id_cliente).ToList(); break;
                            case "cedula": clientesToExport = clientesToExport.OrderBy(c => c.cedula).ToList(); break;
                            case "nombre": clientesToExport = clientesToExport.OrderBy(c => c.nombre).ToList(); break;
                            case "direccion": clientesToExport = clientesToExport.OrderBy(c => c.direccion).ToList(); break;
                            case "telefono": clientesToExport = clientesToExport.OrderBy(c => c.telefono).ToList(); break;
                            case "email": clientesToExport = clientesToExport.OrderBy(c => c.email).ToList(); break;
                            default: clientesToExport = clientesToExport.OrderBy(c => c.nombre).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "id_cliente": clientesToExport = clientesToExport.OrderByDescending(c => c.id_cliente).ToList(); break;
                            case "cedula": clientesToExport = clientesToExport.OrderByDescending(c => c.cedula).ToList(); break;
                            case "nombre": clientesToExport = clientesToExport.OrderByDescending(c => c.nombre).ToList(); break;
                            case "direccion": clientesToExport = clientesToExport.OrderByDescending(c => c.direccion).ToList(); break;
                            case "telefono": clientesToExport = clientesToExport.OrderByDescending(c => c.telefono).ToList(); break;
                            case "email": clientesToExport = clientesToExport.OrderByDescending(c => c.email).ToList(); break;
                            default: clientesToExport = clientesToExport.OrderByDescending(c => c.nombre).ToList(); break;
                        }
                    }
                }
                else
                {
                    clientesToExport = clientesToExport.OrderBy(c => c.nombre).ToList();
                }

                GridView gvExport = new GridView();
                gvExport.DataSource = clientesToExport.Select(c => new { c.id_cliente, c.cedula, c.nombre, c.direccion, c.telefono, c.email }).ToList();
                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Clientes_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
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
                // Expected exception
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar clientes a Excel: {ex.Message}", ex);
                SetMessage($"Ocurrió un error al exportar los datos: {ex.Message}", "error");
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}
