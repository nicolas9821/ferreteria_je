using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.UI;
using System.IO; // Necesario para la exportación a Excel
using MySql.Data.MySqlClient; // Para manejo específico de errores MySQL (ej. foreign key)

namespace ferreteria_je
{
    public partial class ventas : ferreteria_je.session.BasePage
    {
        private VentaRepository _ventaRepository;
        private ClienteRepository _clienteRepository; // Necesario para obtener el nombre del cliente
        private UsuarioRepository _usuarioRepository; // Necesario para obtener el nombre del usuario

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
            _ventaRepository = new VentaRepository();
            _clienteRepository = new ClienteRepository();
            _usuarioRepository = new UsuarioRepository();

            if (!IsPostBack)
            {
                // *** RECOMENDACIÓN: Validación de rol "admin" ***
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarDatosUsuarioEnInterfaz(); // Cargar datos del usuario al cargar la página
                CargarVentas(); // Carga todas las ventas al inicio
            }
        }

        /// <summary>
        /// Carga los datos del usuario logeado en los literales de la interfaz.
        /// </summary>
        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserNameButton.Text = currentUser.nombre; // Nombre en el botón/div del topbar
                litUserFullName.Text = currentUser.nombre;    // Nombre en el dropdown
                litUserEmail.Text = currentUser.email;        // Email en el dropdown
                litUserPhone.Text = currentUser.telefono;     // Teléfono en el dropdown
            }
            else
            {
                // Si no hay usuario en sesión, redirigir al login
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        /// <summary>
        /// Carga las ventas desde la base de datos en el GridView.
        /// Aplica filtrado y ordenación.
        /// </summary>
        private void CargarVentas()
        {
            List<venta> ventasBrutas; // Datos de venta sin nombres de cliente/usuario
            List<cliente> todosLosClientes = _clienteRepository.GetAll().ToList();
            List<usuario> todosLosUsuarios = _usuarioRepository.GetAll().ToList();

            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    if (int.TryParse(searchTerm, out int idVenta))
                    {
                        var ventaEncontrada = _ventaRepository.Get(v => v.id_venta == idVenta);
                        ventasBrutas = (ventaEncontrada != null) ? new List<venta> { ventaEncontrada } : new List<venta>();
                    }
                    else
                    {
                        // Si no es un ID numérico, no se encontrarán resultados con la búsqueda actual basada en ID.
                        // Podrías implementar búsqueda por nombre de cliente/usuario aquí si fuera necesario,
                        // pero la especificación actual es por ID de venta.
                        ventasBrutas = new List<venta>();
                        SetMessage("La búsqueda actual solo soporta ID de venta.", "info");
                    }
                }
                else
                {
                    ventasBrutas = _ventaRepository.GetAll().ToList();
                }

                // Aquí es donde combinamos los datos.
                var datosParaGridView = ventasBrutas.Select(venta => {
                    string nombreCliente = "N/A";
                    if (venta.id_cliente.HasValue)
                    {
                        var cliente = todosLosClientes.FirstOrDefault(c => c.id_cliente == venta.id_cliente.Value);
                        if (cliente != null)
                        {
                            nombreCliente = cliente.nombre;
                        }
                    }

                    string nombreUsuario = "N/A";
                    if (venta.id_usuario.HasValue)
                    {
                        var usuario = todosLosUsuarios.FirstOrDefault(u => u.id_usuario == venta.id_usuario.Value);
                        if (usuario != null)
                        {
                            nombreUsuario = usuario.nombre;
                        }
                    }

                    return new
                    {
                        venta.id_venta,
                        venta.fecha,
                        // Puedes incluir otras propiedades de venta aquí si las necesitas para exportación o display
                        venta.total,
                        NombreCliente = nombreCliente, // Propiedad para el GridView
                        NombreUsuario = nombreUsuario // Propiedad para el GridView
                    };
                }).ToList();

                // Aplicar ordenación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    // Usa un Dynamic LINQ (System.Linq.Dynamic) o un switch para ordenar por nombre de propiedad
                    // Para simplificar, asumiremos las columnas más comunes.
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_venta": datosParaGridView = datosParaGridView.OrderBy(v => v.id_venta).ToList(); break;
                            case "fecha": datosParaGridView = datosParaGridView.OrderBy(v => v.fecha).ToList(); break;
                            case "NombreCliente": datosParaGridView = datosParaGridView.OrderBy(v => v.NombreCliente).ToList(); break;
                            case "NombreUsuario": datosParaGridView = datosParaGridView.OrderBy(v => v.NombreUsuario).ToList(); break;
                            case "total": datosParaGridView = datosParaGridView.OrderBy(v => v.total).ToList(); break;
                            default: datosParaGridView = datosParaGridView.OrderBy(v => v.id_venta).ToList(); break; // Orden por defecto
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_venta": datosParaGridView = datosParaGridView.OrderByDescending(v => v.id_venta).ToList(); break;
                            case "fecha": datosParaGridView = datosParaGridView.OrderByDescending(v => v.fecha).ToList(); break;
                            case "NombreCliente": datosParaGridView = datosParaGridView.OrderByDescending(v => v.NombreCliente).ToList(); break;
                            case "NombreUsuario": datosParaGridView = datosParaGridView.OrderByDescending(v => v.NombreUsuario).ToList(); break;
                            case "total": datosParaGridView = datosParaGridView.OrderByDescending(v => v.total).ToList(); break;
                            default: datosParaGridView = datosParaGridView.OrderByDescending(v => v.id_venta).ToList(); break; // Orden por defecto
                        }
                    }
                }
                else
                {
                    // Orden por defecto si no hay expresión de ordenación
                    datosParaGridView = datosParaGridView.OrderByDescending(v => v.id_venta).ToList(); // Ordenar por ID de venta descendente por defecto
                }


                gvVentas.DataSource = datosParaGridView;
                gvVentas.DataBind();

                if (datosParaGridView.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
                {
                    SetMessage("No se encontraron ventas para el término de búsqueda.", "info");
                }
                else if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    ClearMessages();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar ventas en la página: " + ex.Message, ex);
                SetMessage("Error al cargar las ventas. Consulte los logs para más detalles.", "error");
            }
        }

        /// <summary>
        /// Muestra un mensaje al usuario.
        /// </summary>
        private void SetMessage(string message, string type)
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert hidden";
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden";

            if (type == "success")
            {
                lblMessage.Text = message;
                lblMessage.CssClass = "alert alert-success visible";
            }
            else if (type == "info")
            {
                lblMessage.Text = message;
                lblMessage.CssClass = "alert alert-info visible";
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


        protected void gvVentas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVentas.PageIndex = e.NewPageIndex;
            CargarVentas(); // Llama a CargarVentas sin searchTerm, ya que leerá de txtSearch.Text.Trim()
        }

        protected void gvVentas_Sorting(object sender, GridViewSortEventArgs e)
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
            CargarVentas(); // Recargar con la nueva ordenación
        }

        protected void gvVentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idVenta = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarVenta")
            {
                Response.Redirect($"gestionventas.aspx?id={idVenta}");
            }
            else if (e.CommandName == "EliminarVenta")
            {
                EliminarVenta(idVenta);
                CargarVentas(); // Recarga después de eliminar
            }
        }

        private void EliminarVenta(int idVenta)
        {
            try
            {
                var ventaAEliminar = new venta { id_venta = idVenta };
                _ventaRepository.Delete(ventaAEliminar);
                SetMessage("Venta eliminada correctamente.", "success");
            }
            catch (MySqlException ex) // Captura específica de errores MySQL
            {
                // Código de error 1451 es para violación de clave foránea en MySQL
                if (ex.Number == 1451)
                {
                    SetMessage("No se puede eliminar la venta porque tiene detalles de productos asociados. Elimine los detalles de la venta primero.", "error");
                }
                else
                {
                    Log.Escribir($"Error de BD al eliminar venta con ID {idVenta}: {ex.Message}", ex);
                    SetMessage("Error de base de datos al eliminar la venta. Intente de nuevo o contacte soporte.", "error");
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar venta con ID {idVenta}: {ex.Message}", ex);
                SetMessage($"Error al eliminar la venta con ID {idVenta}. Intente de nuevo más tarde.", "error");
            }
        }

        protected void lnkNuevaVenta_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionventas.aspx");
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvVentas.PageIndex = 0;
            CargarVentas(); // Llama a CargarVentas sin searchTerm, ya que leerá de txtSearch.Text.Trim()
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            gvVentas.PageIndex = 0;
            CargarVentas(); // Llama a CargarVentas sin searchTerm, ya que leerá de txtSearch.Text.Trim()
        }

        // ****** FUNCIONALIDAD DE EXPORTAR A EXCEL ******
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                List<venta> ventasToExportBrutas;
                List<cliente> todosLosClientes = _clienteRepository.GetAll().ToList();
                List<usuario> todosLosUsuarios = _usuarioRepository.GetAll().ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    if (int.TryParse(searchTerm, out int idVenta))
                    {
                        var ventaEncontrada = _ventaRepository.Get(v => v.id_venta == idVenta);
                        ventasToExportBrutas = (ventaEncontrada != null) ? new List<venta> { ventaEncontrada } : new List<venta>();
                    }
                    else
                    {
                        ventasToExportBrutas = new List<venta>();
                    }
                }
                else
                {
                    ventasToExportBrutas = _ventaRepository.GetAll().ToList();
                }

                var datosParaExportar = ventasToExportBrutas.Select(venta => {
                    string nombreCliente = "N/A";
                    if (venta.id_cliente.HasValue)
                    {
                        var cliente = todosLosClientes.FirstOrDefault(c => c.id_cliente == venta.id_cliente.Value);
                        if (cliente != null)
                        {
                            nombreCliente = cliente.nombre;
                        }
                    }

                    string nombreUsuario = "N/A";
                    if (venta.id_usuario.HasValue)
                    {
                        var usuario = todosLosUsuarios.FirstOrDefault(u => u.id_usuario == venta.id_usuario.Value);
                        if (usuario != null)
                        {
                            nombreUsuario = usuario.nombre;
                        }
                    }

                    return new
                    {
                        ID_Venta = venta.id_venta,
                        Fecha = venta.fecha.ToShortDateString(), // Formatear fecha para Excel
                        Cliente = nombreCliente,
                        Usuario = nombreUsuario,
                        Total = venta.total // Sin formato de moneda para Excel
                    };
                }).ToList();

                // Aplicar ordenación a los datos a exportar también
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_venta": datosParaExportar = datosParaExportar.OrderBy(v => v.ID_Venta).ToList(); break;
                            case "fecha": datosParaExportar = datosParaExportar.OrderBy(v => v.Fecha).ToList(); break;
                            case "NombreCliente": datosParaExportar = datosParaExportar.OrderBy(v => v.Cliente).ToList(); break;
                            case "NombreUsuario": datosParaExportar = datosParaExportar.OrderBy(v => v.Usuario).ToList(); break;
                            case "total": datosParaExportar = datosParaExportar.OrderBy(v => v.Total).ToList(); break;
                            default: datosParaExportar = datosParaExportar.OrderBy(v => v.ID_Venta).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "id_venta": datosParaExportar = datosParaExportar.OrderByDescending(v => v.ID_Venta).ToList(); break;
                            case "fecha": datosParaExportar = datosParaExportar.OrderByDescending(v => v.Fecha).ToList(); break;
                            case "NombreCliente": datosParaExportar = datosParaExportar.OrderByDescending(v => v.Cliente).ToList(); break;
                            case "NombreUsuario": datosParaExportar = datosParaExportar.OrderByDescending(v => v.Usuario).ToList(); break;
                            case "total": datosParaExportar = datosParaExportar.OrderByDescending(v => v.Total).ToList(); break;
                            default: datosParaExportar = datosParaExportar.OrderByDescending(v => v.ID_Venta).ToList(); break;
                        }
                    }
                }
                else
                {
                    datosParaExportar = datosParaExportar.OrderByDescending(v => v.ID_Venta).ToList();
                }

                GridView gvExport = new GridView();
                gvExport.DataSource = datosParaExportar;
                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Ventas_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
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
                // Esta excepción es normal cuando se usa Response.End()
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar ventas a Excel: {ex.Message}", ex);
                SetMessage($"Ocurrió un error al exportar los datos: {ex.Message}", "error");
            }
        }

        // Método necesario para que RenderControl funcione correctamente al exportar.
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}