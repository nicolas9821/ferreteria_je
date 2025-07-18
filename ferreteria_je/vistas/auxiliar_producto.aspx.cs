using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web; // Necesario para HttpResponse, HttpContext
using System.Web.UI.WebControls; // Para GridView, etc.
using System.IO; // Necesario para StringWriter
using System.Web.UI; // Necesario para HtmlTextWriter (para VerifyRenderingInServerForm y RenderControl)

// Importar los namespaces de tu proyecto
using ferreteria_je.Repositories.Models; // Tu modelo 'producto'
using ferreteria_je.Repositories.Interfaces; // Tu interfaz IProductoRepository
using ferreteria_je.Repositories; // Donde está ProductoRepository (la implementación concreta)
using ferreteria_je.Utilidades; // Tu clase Log (para el manejo de errores)
using ferreteria_je.session; // Asegúrate de que esta sea la ruta correcta a tu BasePage

namespace ferreteria_je
{
    public partial class auxiliar_producto : ferreteria_je.session.BasePage
    {
        private IProductoRepository _productoRepository;
        private IUsuarioRepository _usuarioRepository; // Necesario para obtener datos del usuario logeado

        protected void Page_Init(object sender, EventArgs e)
        {
            // Inicializar los repositorios aquí para que estén disponibles durante todo el ciclo de vida
            _productoRepository = new ProductoRepository();
            _usuarioRepository = new UsuarioRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Solo cargar datos la primera vez que la página se carga
            {
                // ** Validar Rol del Auxiliar (o Administrador si lo permites aquí) **
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "auxiliar")
                {
                    Response.Redirect("~/vistas/login.aspx"); // Redirigir si no es auxiliar
                    return; // Importante para detener la ejecución
                }

                CargarDatosUsuarioEnInterfaz(); // Carga los datos del usuario logeado en los literales
                CheckUserRolePermissions(); // Verifica y ajusta la visibilidad de los controles según el rol
                CargarProductos(); // Carga los datos iniciales de los productos
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

        /// <summary>
        /// Verifica el rol del usuario actual y ajusta la visibilidad de los controles
        /// como el botón de "Agregar producto".
        /// </summary>
        private void CheckUserRolePermissions()
        {
            if (Session["UserRole"] != null)
            {
                string currentUserRoleName = Session["UserRole"].ToString().ToLower();

                // El botón 'Agregar Producto' será visible SOLO para "auxiliar" y "administrador"
                btnAddProducto.Visible = (currentUserRoleName == "auxiliar" || currentUserRoleName == "administrador");

                // La lógica para los botones de editar/eliminar en el GridView se maneja en RowDataBound.
            }
            else
            {
                // Si no hay sesión, ocultar el botón de agregar
                btnAddProducto.Visible = false;
            }
        }

        /// <summary>
        /// Maneja el evento RowDataBound del GridView.
        /// Oculta los botones de acción (Editar/Eliminar) si el usuario no es "administrador".
        /// </summary>
        protected void gvProductos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string role = Session["UserRole"]?.ToString().ToLower();
                if (role != "administrador") // Solo el administrador debería ver estos botones
                {
                    // Encuentra los botones y ocúltalos
                    LinkButton btnEdit = (LinkButton)e.Row.FindControl("btnEditar");
                    LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnEliminar");

                    if (btnEdit != null) btnEdit.Visible = false;
                    if (btnDelete != null) btnDelete.Visible = false;
                }
            }
        }

        /// <summary>
        /// Carga la lista de productos desde el repositorio y la enlaza al GridView.
        /// Aplica el filtro de búsqueda si se proporciona un término.
        /// </summary>
        private void CargarProductos()
        {
            try
            {
                List<producto> productos;
                string searchTerm = txtBuscarProducto.Text.Trim(); // Obtiene el término de búsqueda

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // Si hay un término de búsqueda, usa el método para filtrar por nombre
                    productos = _productoRepository.GetProductosByNombre(searchTerm);
                }
                else
                {
                    // Si no hay término de búsqueda, obtén todos los productos
                    productos = _productoRepository.GetAll().ToList();
                }

                if (productos != null && productos.Any())
                {
                    gvProductos.DataSource = productos;
                    gvProductos.DataBind();
                    lblMessage.Text = $"Se han cargado {productos.Count} productos.";
                    lblMessage.CssClass = "alert alert-info visible"; // Hacerlo visible
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden"; // Ocultarlo
                }
                else
                {
                    gvProductos.DataSource = null; // Para mostrar EmptyDataTemplate
                    gvProductos.DataBind();
                    lblMessage.Text = "No se encontraron productos.";
                    lblMessage.CssClass = "alert alert-info visible"; // Hacerlo visible
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden"; // Ocultarlo
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar la lista de productos: {ex.Message}", ex);
                lblErrorMessage.Text = "Ocurrió un error al cargar los productos. Por favor, intente de nuevo más tarde.";
                lblErrorMessage.CssClass = "alert alert-danger visible"; // Hacerlo visible
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden"; // Ocultarlo
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de página del GridView.
        /// </summary>
        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            CargarProductos(); // Vuelve a cargar los datos para la nueva página
        }

        /// <summary>
        /// Maneja el clic en el botón de búsqueda.
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0; // Al buscar, siempre regresar a la primera página
            CargarProductos(); // Vuelve a cargar los productos aplicando el filtro
        }

        /// <summary>
        /// Maneja el clic en el botón "Exportar a Excel".
        /// Exporta los datos actualmente filtrados (o todos si no hay filtro) del GridView a un archivo Excel.
        /// </summary>
        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Temporalmente, ajustar la configuración del GridView para la exportación
                gvProductos.AllowPaging = false;     // Deshabilitar la paginación para exportar todos los datos (filtrados o no)
                gvProductos.EnableViewState = false; // Deshabilitar ViewState para una exportación más limpia

                // Recargar los datos con el filtro actual si lo hay, para que el GridView tenga los datos correctos a exportar
                CargarProductos();

                // Ocultar la columna de "Acciones" y limpiar los estilos CSS para una exportación limpia
                foreach (DataControlField column in gvProductos.Columns)
                {
                    if (column.HeaderText == "Acciones") // Identifica por HeaderText
                    {
                        column.Visible = false; // Oculta la columna de acciones
                    }
                    // Quitar estilos CSS para que el Excel no se rompa
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
                // Quitar estilos CSS directamente del GridView también
                gvProductos.HeaderStyle.CssClass = "";
                gvProductos.RowStyle.CssClass = "";
                gvProductos.AlternatingRowStyle.CssClass = "";
                gvProductos.PagerStyle.CssClass = "";
                gvProductos.FooterStyle.CssClass = "";


                // CREAR UN PANEL TEMPORAL PARA RENDERIZAR SOLO EL GRIDVIEW
                // Esto es CRUCIAL para evitar el error "Los métodos de enlace de datos..."
                Panel p = new Panel();
                p.Controls.Add(gvProductos); // Mueve el GridView al Panel temporal

                // Configurar la respuesta HTTP para la descarga del archivo Excel
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Productos_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "UTF-8"; // Usar UTF-8 para evitar problemas con caracteres especiales
                Response.ContentEncoding = System.Text.Encoding.UTF8; // Asegurar la codificación

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        // Renderiza el Panel (que contiene el GridView) al HtmlTextWriter
                        p.RenderControl(hw);
                        Response.Write(sw.ToString());
                    }
                }

                Response.Flush();
                Response.End(); // Termina la respuesta HTTP y envía el archivo al navegador
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
                Log.Escribir($"Error al exportar productos a Excel: {ex.Message}", ex);
                lblErrorMessage.Text = "Ocurrió un error al exportar los datos. Por favor, intente de nuevo.";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
            finally
            {
                // NOTA: Este bloque 'finally' NO se ejecutará si 'Response.End()'
                // se llamó con éxito, ya que el procesamiento del hilo se detiene.
                // La restauración de la paginación y la visibilidad de la columna
                // de acciones se manejará en la próxima carga de la página (Page_Load).

                // Asegurarse de volver a hacer visible la columna de acciones (importante para la visualización posterior en la página)
                // Esta parte solo se ejecutaría si Response.End() fallara o no se llamara.
                // En un escenario normal de exportación exitosa, esta lógica se 'salta'.
                // gvProductos.AllowPaging = true;
                // gvProductos.EnableViewState = true;
                // foreach (DataControlField column in gvProductos.Columns)
                // {
                //     if (column.HeaderText == "Acciones")
                //     {
                //         column.Visible = true;
                //         break; 
                //     }
                // }
                // CargarProductos(); // Para restaurar la paginación normal si no se hizo Response.End()
            }
        }

        /// <summary>
        /// Sobrescribe este método para permitir que los controles se rendericen directamente
        /// en la respuesta HTTP (necesario para la exportación a Excel).
        /// </summary>
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Se deja vacío intencionadamente.
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Lógica para cerrar la sesión del usuario.
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }
    }
}