using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories;
using ferreteria_je.Utilidades;
using ferreteria_je.session; // Asegúrate de que esta sea la ruta correcta a tu BasePage

namespace ferreteria_je
{
    public partial class auxiliar_proveedor : ferreteria_je.session.BasePage // Heredar de BasePage
    {
        private IProveedorRepository _proveedorRepository;
        private IUsuarioRepository _usuarioRepository; // Necesario para obtener datos del usuario logeado

        protected void Page_Init(object sender, EventArgs e)
        {
            // Inicializar los repositorios aquí
            _proveedorRepository = new ProveedorRepository();
            _usuarioRepository = new UsuarioRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ** Validar Rol del Auxiliar **
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "auxiliar")
                {
                    Response.Redirect("~/vistas/login.aspx"); // Redirigir si no es auxiliar
                    return; // Importante para detener la ejecución
                }

                CheckUserRolePermissions();
                CargarProveedores();
                CargarDatosUsuarioEnInterfaz(); // Llama a este nuevo método
            }
        }

        private void CargarDatosUsuarioEnInterfaz()
        {
            // Verifica si el objeto de usuario está en la sesión y es del tipo correcto
            if (Session["usuario"] is usuario currentUser)
            {
                // Asigna los valores a los Literal controls en el ASPX
                litUserName.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                // Si el usuario no está en sesión o el objeto no es válido, redirigir al login
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        private void CheckUserRolePermissions()
        {
            if (Session["UserRole"] != null)
            {
                string currentUserRoleName = Session["UserRole"].ToString().ToLower();

                // El botón 'Agregar Proveedor' será visible SOLO para "administrador"
                btnAddProveedor.Visible = (currentUserRoleName == "administrador");

                // La visibilidad de los botones de edición/eliminación dentro del GridView
                // se maneja en el evento gvProveedores_RowDataBound.
            }
            else
            {
                // Si no hay sesión (usuario no logueado), ocultar el botón de agregar
                btnAddProveedor.Visible = false;
            }
        }

        protected void gvProveedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Ocultar botones de acción para el rol de "auxiliar"
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string role = Session["UserRole"]?.ToString().ToLower();
                if (role != "administrador")
                {
                    // Encuentra los botones LinkButton por su ID y ocúltalos
                    LinkButton btnEdit = (LinkButton)e.Row.FindControl("btnEditar");
                    LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnEliminar");
                    LinkButton btnViewDetails = (LinkButton)e.Row.FindControl("btnVerDetalles");

                    if (btnEdit != null) btnEdit.Visible = false;
                    if (btnDelete != null) btnDelete.Visible = false;
                    // Si el auxiliar solo debe VER, este no lo ocultamos:
                    // if (btnViewDetails != null) btnViewDetails.Visible = false; 
                    // En este caso, el botón de ver detalles debería seguir siendo visible para el auxiliar
                }
            }
        }


        private void CargarProveedores()
        {
            try
            {
                List<proveedor> proveedores;
                string searchTerm = txtBuscarProveedor.Text.Trim();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // Obtiene proveedores filtrados por nombre
                    proveedores = _proveedorRepository.GetProveedoresByNombre(searchTerm);
                }
                else
                {
                    // Obtiene todos los proveedores
                    proveedores = _proveedorRepository.GetAll().ToList();
                }

                if (proveedores != null && proveedores.Any())
                {
                    gvProveedores.DataSource = proveedores;
                    gvProveedores.DataBind();
                    lblMessage.Text = $"Se han cargado {proveedores.Count} proveedores.";
                    lblMessage.CssClass = "alert alert-info visible"; // Muestra el mensaje informativo
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden"; // Oculta el mensaje de error
                }
                else
                {
                    gvProveedores.DataSource = null;
                    gvProveedores.DataBind();
                    lblMessage.Text = "No se encontraron proveedores.";
                    lblMessage.CssClass = "alert alert-info visible"; // Muestra el mensaje informativo
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden"; // Oculta el mensaje de error
                }
            }
            catch (Exception ex)
            {
                // Registra el error en el log
                Log.Escribir($"Error al cargar la lista de proveedores: {ex.Message}", ex);
                lblErrorMessage.Text = "Ocurrió un error al cargar los proveedores. Por favor, intente de nuevo más tarde.";
                lblErrorMessage.CssClass = "alert alert-danger visible"; // Muestra el mensaje de error
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden"; // Oculta el mensaje informativo
            }
        }

        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;
            CargarProveedores(); // Recarga los datos para la nueva página
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarProveedores(); // Vuelve a cargar los proveedores aplicando el filtro de búsqueda
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Deshabilitar paginación y ViewState TEMPORALMENTE para exportar todos los datos sin problemas
                gvProveedores.AllowPaging = false;
                gvProveedores.EnableViewState = false;

                // Cargar los datos que se van a exportar. 
                // Esto asegura que el GridView contenga los datos correctos (filtrados o todos)
                // ANTES de la renderización para Excel.
                CargarProveedores();

                // Ocultar la columna de "Acciones" y limpiar los estilos CSS del GridView y sus columnas
                // para que el archivo Excel sea lo más limpio posible.
                foreach (DataControlField column in gvProveedores.Columns)
                {
                    if (column.HeaderText == "Acciones") // Identificar la columna por su HeaderText
                    {
                        column.Visible = false; // Ocultar la columna de acciones
                    }
                    // Quitar estilos CSS de las cabeceras e ítems de las columnas
                    if (column is BoundField bf)
                    {
                        bf.HeaderStyle.CssClass = "";
                        bf.ItemStyle.CssClass = "";
                    }
                    else if (column is TemplateField tf)
                    {
                        tf.HeaderStyle.CssClass = "";
                        tf.ItemStyle.CssClass = "";
                    }
                }

                // Quitar estilos CSS del propio GridView
                gvProveedores.HeaderStyle.CssClass = "";
                gvProveedores.RowStyle.CssClass = "";
                gvProveedores.AlternatingRowStyle.CssClass = "";
                gvProveedores.PagerStyle.CssClass = "";
                gvProveedores.FooterStyle.CssClass = "";


                // Crear un Panel temporal para contener y renderizar solo el GridView.
                // Esto es CRUCIAL para evitar la System.InvalidOperationException.
                Panel panelToRender = new Panel();
                panelToRender.Controls.Add(gvProveedores); // Mover el GridView al panel temporal

                // Configurar la respuesta HTTP para la descarga del archivo Excel
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Proveedores_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "UTF-8"; // Usar UTF-8 para una mejor compatibilidad con caracteres especiales
                Response.ContentEncoding = System.Text.Encoding.UTF8; // Asegurar la codificación

                // Renderizar el Panel (que ahora contiene el GridView) a un StringWriter.
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        panelToRender.RenderControl(hw); // Renderiza el panel, y con ello, el GridView
                        Response.Write(sw.ToString()); // Escribe el HTML renderizado en la respuesta
                    }
                }

                // Finalizar la respuesta HTTP para enviar el archivo al navegador.
                Response.Flush(); // Asegura que todo el contenido se envíe
                Response.End();   // Termina la solicitud actual y evita el procesamiento adicional de la página.

            }
            catch (System.Threading.ThreadAbortException)
            {
                // Esta excepción es normal y esperada cuando se llama a Response.End()
                // No es un error real, solo indica que el hilo de procesamiento de la página
                // ha sido abortado intencionadamente para enviar el archivo.
                // No es necesario loguearla como un error.
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción inesperada durante la exportación
                Log.Escribir($"Error al exportar proveedores a Excel: {ex.Message}", ex);
                lblErrorMessage.Text = "Ocurrió un error al exportar los datos. Por favor, intente de nuevo.";
                lblErrorMessage.CssClass = "alert alert-danger visible"; // Mostrar mensaje de error
                lblMessage.Text = string.Empty; // Limpiar mensaje informativo
                lblMessage.CssClass = "alert alert-info hidden"; // Ocultar mensaje informativo
            }
            finally
            {
                // **IMPORTANTE**: Este bloque 'finally' NO se ejecutará si 'Response.End()'
                // se llama con éxito, ya que el procesamiento del hilo se detiene abruptamente.
                // Por lo tanto, la restauración de la paginación y la visibilidad de la columna
                // de acciones debe ocurrir en el Page_Load o en una recarga posterior de la página.
                // Para propósitos de depuración o si Response.End() no siempre funciona como se espera
                // (por ejemplo, en servidores específicos o configuraciones de IIS), podrías poner
                // estas restauraciones aquí, pero ten en cuenta que podrían no ejecutarse.

                // Como la página se carga de nuevo después de la exportación (al cerrar la descarga o navegar),
                // el Page_Load se encargará de restaurar el estado normal del GridView.
            }
        }

        // Este método es crucial para la exportación y debe estar presente en el code-behind.
        // Permite que el GridView se renderice directamente en el HttpResponse
        // sin estar dentro de una etiqueta <form runat="server">, que es lo que ASP.NET
        // normalmente requiere para los controles que tienen enlace de datos.
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Se deja vacío intencionadamente.
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Limpia la sesión y redirige al usuario a la página de login.
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }
    }
}