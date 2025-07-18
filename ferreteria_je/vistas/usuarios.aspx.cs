// ferreteria_je\usuarios.aspx.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System.IO; // Necesario para StringWriter y HtmlTextWriter
using MySql.Data.MySqlClient; // Para manejo específico de errores MySQL (ej. foreign key)

namespace ferreteria_je
{
    public partial class usuarios : ferreteria_je.session.BasePage
    {
        private UsuarioRepository _usuarioRepository;

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
            _usuarioRepository = new UsuarioRepository();

            if (!IsPostBack)
            {
                // RECOMENDACIÓN: Validación de rol "admin"
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
                {
                    Response.Redirect("~/vistas/login.aspx"); // Redirigir si no es admin
                    return; // Detener la ejecución de la página
                }

                CargarDatosUsuarioEnInterfaz(); // Cargar datos del usuario logeado en los literales
                CargarUsuarios(); // Carga todos los usuarios al inicio
                // No llamar a ClearMessages aquí si CargarUsuarios ya lo hace o si queremos que persistan los mensajes iniciales.
                // La lógica de mensajes se gestionará dentro de CargarUsuarios y EliminarUsuario.
            }
        }

        /// <summary>
        /// Carga los datos del usuario logeado en los literales de la interfaz (topbar).
        /// </summary>
        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserNameButton.Text = currentUser.nombre; // Nombre en el botón/div del topbar
                litUserFullName.Text = currentUser.nombre;    // Nombre en el dropdown
                litUserEmail.Text = currentUser.email;        // Email en el dropdown
                litUserPhone.Text = currentUser.telefono;      // Teléfono en el dropdown
            }
            else
            {
                // Si no hay usuario en sesión, redirigir al login (aunque ya lo hace la validación de rol)
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        /// <summary>
        /// Carga los usuarios desde la base de datos en el GridView.
        /// Aplica filtrado y ordenación.
        /// </summary>
        private void CargarUsuarios()
        {
            List<ferreteria_je.Repositories.Models.usuario> listaUsuarios;
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    listaUsuarios = _usuarioRepository.GetUsuariosByNombre(searchTerm);
                    if (listaUsuarios == null || listaUsuarios.Count == 0)
                    {
                        // Si la búsqueda por nombre no encuentra nada, intenta buscar por ID si es un número
                        if (int.TryParse(searchTerm, out int idUsuario))
                        {
                            var usuarioById = _usuarioRepository.Get(u => u.id_usuario == idUsuario);
                            if (usuarioById != null)
                            {
                                listaUsuarios = new List<ferreteria_je.Repositories.Models.usuario> { usuarioById };
                            }
                            else
                            {
                                listaUsuarios = new List<ferreteria_je.Repositories.Models.usuario>();
                            }
                        }
                        else
                        {
                            listaUsuarios = new List<ferreteria_je.Repositories.Models.usuario>(); // No es nombre ni ID válido
                        }
                    }
                }
                else
                {
                    listaUsuarios = _usuarioRepository.GetAll().ToList();
                }

                // Aplicar ordenación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_usuario": listaUsuarios = listaUsuarios.OrderBy(u => u.id_usuario).ToList(); break;
                            case "nombre": listaUsuarios = listaUsuarios.OrderBy(u => u.nombre).ToList(); break;
                            case "telefono": listaUsuarios = listaUsuarios.OrderBy(u => u.telefono).ToList(); break;
                            case "email": listaUsuarios = listaUsuarios.OrderBy(u => u.email).ToList(); break;
                            case "rol": listaUsuarios = listaUsuarios.OrderBy(u => u.rol).ToList(); break;
                            default: listaUsuarios = listaUsuarios.OrderBy(u => u.nombre).ToList(); break; // Orden por defecto
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_usuario": listaUsuarios = listaUsuarios.OrderByDescending(u => u.id_usuario).ToList(); break;
                            case "nombre": listaUsuarios = listaUsuarios.OrderByDescending(u => u.nombre).ToList(); break;
                            case "telefono": listaUsuarios = listaUsuarios.OrderByDescending(u => u.telefono).ToList(); break;
                            case "email": listaUsuarios = listaUsuarios.OrderByDescending(u => u.email).ToList(); break;
                            case "rol": listaUsuarios = listaUsuarios.OrderByDescending(u => u.rol).ToList(); break;
                            default: listaUsuarios = listaUsuarios.OrderByDescending(u => u.nombre).ToList(); break; // Orden por defecto
                        }
                    }
                }
                else
                {
                    // Orden por defecto si no hay expresión de ordenación
                    listaUsuarios = listaUsuarios.OrderBy(u => u.nombre).ToList();
                }

                gvUsuarios.DataSource = listaUsuarios;
                gvUsuarios.DataBind();

                // ¡¡¡AJUSTE CLAVE AQUÍ!!!
                // Solo actualiza los mensajes de éxito si NO hay un mensaje de error activo.
                // Si lblErrorMessage.Text tiene contenido, significa que EliminarUsuario ya puso un error,
                // y CargarUsuarios no debe sobrescribirlo con el mensaje de éxito.
                if (string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    if (listaUsuarios.Count == 0 && !string.IsNullOrWhiteSpace(searchTerm))
                    {
                        SetMessage("No se encontraron usuarios con el término de búsqueda.", "info");
                    }
                    else if (listaUsuarios.Count > 0)
                    {
                        SetMessage($"Se han cargado {listaUsuarios.Count()} usuarios.", "info");
                    }
                    else // Si no hay usuarios y no hay término de búsqueda (tabla vacía)
                    {
                        SetMessage("No se encontraron usuarios.", "info");
                    }
                }
                // Si lblErrorMessage.Text NO está vacío, no hacemos nada aquí para preservar el mensaje de error.
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar usuarios en la página: " + ex.Message, ex);
                SetMessage("Error al cargar los usuarios. Consulte los logs para más detalles.", "error");
            }
        }

        /// <summary>
        /// Muestra un mensaje al usuario.
        /// </summary>
        private void SetMessage(string message, string type)
        {
            // Oculta ambos mensajes primero para asegurar que solo se muestre uno
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

        protected void gvUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsuarios.PageIndex = e.NewPageIndex;
            CargarUsuarios(); // Recarga con la nueva página, manteniendo el filtro si existe
        }

        protected void gvUsuarios_Sorting(object sender, GridViewSortEventArgs e)
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
            CargarUsuarios(); // Recargar con la nueva ordenación
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idUsuario = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarUsuario")
            {
                Response.Redirect($"gestionusuarios.aspx?id={idUsuario}");
            }
            else if (e.CommandName == "EliminarUsuario")
            {
                EliminarUsuario(idUsuario);
                CargarUsuarios(); // Recarga después de eliminar
            }
        }

        private void EliminarUsuario(int idUsuario)
        {
            ClearMessages(); // ¡¡¡NUEVO!!! Limpia los mensajes antes de intentar eliminar

            try
            {
                var usuarioAEliminar = new ferreteria_je.Repositories.Models.usuario { id_usuario = idUsuario };
                _usuarioRepository.Delete(usuarioAEliminar);
                SetMessage("Usuario eliminado exitosamente.", "success");
            }
            catch (Exception ex) // Captura general para cualquier excepción
            {
                Log.Escribir($"Error al eliminar usuario con ID {idUsuario}: {ex.Message}", ex);

                string userFriendlyErrorMessage = "Error al eliminar el usuario. Intente de nuevo más tarde.";

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
                            userFriendlyErrorMessage = $"No se puede eliminar el usuario con ID {idUsuario} porque tiene registros asociados (ej. ventas, compras). Elimine los registros asociados primero.";
                            break; // Error específico encontrado, salir del bucle
                        }
                    }
                    // Si usas un ORM (como Entity Framework) con MySQL, la MySqlException podría estar anidada
                    // Por ejemplo, si la excepción inicial es DbUpdateException y dentro tiene la MySqlException:
                    // if (currentEx is System.Data.Entity.Infrastructure.DbUpdateException dbUpdateEx)
                    // {
                    //     if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1451)
                    //     {
                    //         userFriendlyErrorMessage = $"No se puede eliminar el usuario con ID {idUsuario} porque tiene registros asociados (ej. ventas, compras). Elimine los registros asociados primero.";
                    //         break;
                    //     }
                    // }

                    currentEx = currentEx.InnerException;
                }

                SetMessage(userFriendlyErrorMessage, "error"); // Usa SetMessage para mostrar el error
            }
        }

        protected void lnkAgregarUsuario_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionusuarios.aspx");
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvUsuarios.PageIndex = 0; // Reiniciar página al buscar
            CargarUsuarios(); // La función CargarUsuarios leerá el texto del cuadro de búsqueda
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            gvUsuarios.PageIndex = 0; // Reiniciar página al cambiar texto y autopostback
            CargarUsuarios(); // La función CargarUsuarios leerá el texto del cuadro de búsqueda
        }

        // FUNCIONALIDAD DE EXPORTAR A EXCEL
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                List<ferreteria_je.Repositories.Models.usuario> usuariosToExport;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    usuariosToExport = _usuarioRepository.GetUsuariosByNombre(searchTerm);
                    if (usuariosToExport == null || usuariosToExport.Count == 0)
                    {
                        if (int.TryParse(searchTerm, out int idUsuario))
                        {
                            var usuarioById = _usuarioRepository.Get(u => u.id_usuario == idUsuario);
                            if (usuarioById != null)
                            {
                                usuariosToExport = new List<ferreteria_je.Repositories.Models.usuario> { usuarioById };
                            }
                            else
                            {
                                usuariosToExport = new List<ferreteria_je.Repositories.Models.usuario>();
                            }
                        }
                        else
                        {
                            usuariosToExport = new List<ferreteria_je.Repositories.Models.usuario>();
                        }
                    }
                }
                else
                {
                    usuariosToExport = _usuarioRepository.GetAll().ToList();
                }

                // Aplicar ordenación a los datos a exportar también
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_usuario": usuariosToExport = usuariosToExport.OrderBy(u => u.id_usuario).ToList(); break;
                            case "nombre": usuariosToExport = usuariosToExport.OrderBy(u => u.nombre).ToList(); break;
                            case "telefono": usuariosToExport = usuariosToExport.OrderBy(u => u.telefono).ToList(); break;
                            case "email": usuariosToExport = usuariosToExport.OrderBy(u => u.email).ToList(); break;
                            case "rol": usuariosToExport = usuariosToExport.OrderBy(u => u.rol).ToList(); break;
                            default: usuariosToExport = usuariosToExport.OrderBy(u => u.nombre).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "id_usuario": usuariosToExport = usuariosToExport.OrderByDescending(u => u.id_usuario).ToList(); break;
                            case "nombre": usuariosToExport = usuariosToExport.OrderByDescending(u => u.nombre).ToList(); break;
                            case "telefono": usuariosToExport = usuariosToExport.OrderByDescending(u => u.telefono).ToList(); break;
                            case "email": usuariosToExport = usuariosToExport.OrderByDescending(u => u.email).ToList(); break;
                            case "rol": usuariosToExport = usuariosToExport.OrderByDescending(u => u.rol).ToList(); break;
                            default: usuariosToExport = usuariosToExport.OrderByDescending(u => u.nombre).ToList(); break;
                        }
                    }
                }
                else
                {
                    usuariosToExport = usuariosToExport.OrderBy(u => u.nombre).ToList(); // Ordenar por defecto si no hay expresión de ordenación
                }


                // Crear un GridView temporal para la exportación
                GridView gvExport = new GridView();
                gvExport.DataSource = usuariosToExport.Select(u => new { u.id_usuario, u.nombre, u.telefono, u.email, u.rol }).ToList();
                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Usuarios_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        // Renderiza el GridView en el HtmlTextWriter
                        gvExport.RenderControl(hw);
                        // Escribe el contenido HTML al Stream
                        Response.Write(sw.ToString());
                    }
                }

                Response.Flush();
                Response.End(); // Finaliza la respuesta para enviar el archivo
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Esta excepción es normal y esperada cuando se usa Response.End()
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar usuarios a Excel: {ex.Message}", ex);
                SetMessage($"Ocurrió un error al exportar los datos: {ex.Message}", "error");
            }
        }

        // Método necesario para que RenderControl funcione correctamente al exportar.
        // Debe estar vacío.
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}
