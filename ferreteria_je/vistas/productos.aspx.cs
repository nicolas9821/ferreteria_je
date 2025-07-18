using System;
using System.Collections.Generic;
using System.IO; // Necesario para StringWriter y HtmlTextWriter
using System.Linq; // Necesario para LINQ (Where, OrderBy, ToList)
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories; // Importa tu ProductoRepository
using ferreteria_je.Repositories.Models; // Importa tu modelo 'producto'
using ferreteria_je.Utilidades; // Importa tu clase Log (para manejo de errores)
using ferreteria_je.session; // Asegúrate de tener esta importación si BasePage está en esa ruta
using MySql.Data.MySqlClient; // Para manejo específico de errores MySQL (ej. foreign key)

namespace ferreteria_je
{
    public partial class productos : ferreteria_je.session.BasePage
    {
        private ProductoRepository _productoRepository;

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
            _productoRepository = new ProductoRepository();

            if (!IsPostBack)
            {
                // *** RECOMENDACIÓN: Validación de rol "admin" ***
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarDatosUsuarioEnInterfaz(); // Cargar datos del usuario al cargar la página
                CargarProductos(); // Carga todos los productos al inicio
                ClearMessages(); // Limpiar mensajes en la carga inicial
            }
        }

        /// <summary>
        /// Método auxiliar para limpiar todos los mensajes de estado y error.
        /// </summary>
        private void ClearMessages()
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert alert-info hidden"; // Asegura que esté oculto
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden"; // Asegura que esté oculto
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
        /// Carga los productos desde la base de datos en el GridView.
        /// Aplica filtrado y ordenación.
        /// Se ha modificado para obtener el stock de cada producto utilizando el SP ObtenerStockProducto.
        /// </summary>
        private void CargarProductos()
        {
            List<producto> listaProductos = new List<producto>();
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                // Obtener todos los productos inicialmente (esto puede usar Dapper o tu GenericRepository)
                listaProductos = _productoRepository.GetAll().ToList();

                // *** CAMBIO CLAVE AQUÍ: Obtener el stock de cada producto usando el SP ObtenerStockProducto ***
                // Esto demuestra el uso del parámetro OUT
                foreach (var prod in listaProductos)
                {
                    try
                    {
                        prod.stock = _productoRepository.ObtenerStockProductoViaSP(prod.id_producto);
                    }
                    catch (Exception exStock)
                    {
                        // Si falla al obtener el stock de un producto específico, loggearlo pero no detener la carga.
                        Log.Escribir($"Error al obtener stock para producto ID {prod.id_producto} vía SP: {exStock.Message}", exStock);
                        prod.stock = -1; // Indicar un stock desconocido o error
                    }
                }


                // Aplicar filtro si hay un término de búsqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    listaProductos = listaProductos
                        .Where(p => p.nombre.ToLower().Contains(searchTerm.ToLower()) ||
                                     p.descripcion.ToLower().Contains(searchTerm.ToLower())) // Ajusta según campos de búsqueda
                        .ToList();
                }

                // Aplicar ordenación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_producto": listaProductos = listaProductos.OrderBy(p => p.id_producto).ToList(); break;
                            case "nombre": listaProductos = listaProductos.OrderBy(p => p.nombre).ToList(); break;
                            case "descripcion": listaProductos = listaProductos.OrderBy(p => p.descripcion).ToList(); break;
                            case "precio": listaProductos = listaProductos.OrderBy(p => p.precio).ToList(); break;
                            case "stock": listaProductos = listaProductos.OrderBy(p => p.stock).ToList(); break;
                            default: listaProductos = listaProductos.OrderBy(p => p.id_producto).ToList(); break; // Orden por defecto
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_producto": listaProductos = listaProductos.OrderByDescending(p => p.id_producto).ToList(); break;
                            case "nombre": listaProductos = listaProductos.OrderByDescending(p => p.nombre).ToList(); break;
                            case "descripcion": listaProductos = listaProductos.OrderByDescending(p => p.descripcion).ToList(); break;
                            case "precio": listaProductos = listaProductos.OrderByDescending(p => p.precio).ToList(); break;
                            case "stock": listaProductos = listaProductos.OrderByDescending(p => p.stock).ToList(); break;
                            default: listaProductos = listaProductos.OrderByDescending(p => p.id_producto).ToList(); break; // Orden por defecto
                        }
                    }
                }
                else
                {
                    // Orden por defecto si no hay expresión de ordenación
                    listaProductos = listaProductos.OrderBy(p => p.id_producto).ToList();
                }

                gvProductos.DataSource = listaProductos;
                gvProductos.DataBind();

                // ¡¡¡AJUSTE CLAVE AQUÍ: Solo actualiza los mensajes de éxito si no hay un mensaje de error activo!!!
                if (string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    if (listaProductos != null && listaProductos.Any())
                    {
                        lblMessage.Text = $"Se han cargado {listaProductos.Count()} productos.";
                        lblMessage.CssClass = "alert alert-info visible";
                    }
                    else
                    {
                        lblMessage.Text = "No se encontraron productos.";
                        lblMessage.CssClass = "alert alert-info visible";
                    }
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden";
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar productos: {ex.Message}", ex);
                lblErrorMessage.Text = "No se pudieron cargar los productos. Intente de nuevo más tarde.";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de página del GridView.
        /// </summary>
        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            CargarProductos();
        }

        /// <summary>
        /// Maneja el evento de ordenación del GridView.
        /// </summary>
        protected void gvProductos_Sorting(object sender, GridViewSortEventArgs e)
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
            CargarProductos();
        }

        /// <summary>
        /// Maneja los comandos (como Editar o Eliminar) enviados desde los botones dentro de las filas del GridView.
        /// </summary>
        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditarProducto")
            {
                int idProducto = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"gestionproductos.aspx?id={idProducto}");
            }
            else if (e.CommandName == "EliminarProducto")
            {
                int idProducto = Convert.ToInt32(e.CommandArgument);
                EliminarProducto(idProducto);
                CargarProductos();
            }
        }

        /// <summary>
        /// Elimina un producto de la base de datos.
        /// </summary>
        private void EliminarProducto(int idProducto)
        {
            ClearMessages();

            try
            {
                var productoAEliminar = new producto { id_producto = idProducto };
                _productoRepository.Delete(productoAEliminar);
                lblMessage.Text = $"Producto con ID {idProducto} eliminado correctamente.";
                lblMessage.CssClass = "alert alert-success visible";
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar producto con ID {idProducto}: {ex.Message}", ex);

                string userFriendlyErrorMessage = "Error al eliminar el producto. Intente de nuevo más tarde.";

                Exception currentEx = ex;
                while (currentEx != null)
                {
                    if (currentEx is MySqlException mySqlEx)
                    {
                        if (mySqlEx.Number == 1451)
                        {
                            userFriendlyErrorMessage = $"No se puede eliminar el producto con ID {idProducto} porque tiene registros asociados (ej. en ventas, compras). Elimine los registros relacionados primero.";
                            break;
                        }
                    }
                    currentEx = currentEx.InnerException;
                }

                lblErrorMessage.Text = userFriendlyErrorMessage;
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        /// <summary>
        /// Maneja el clic del botón "Agregar producto".
        /// </summary>
        protected void lnkAgregarProducto_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionproductos.aspx");
        }

        /// <summary>
        /// Maneja el clic del botón de búsqueda (btnSearch).
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0;
            CargarProductos();
        }

        /// <summary>
        /// Maneja el clic del botón "Cerrar sesión".
        /// </summary>
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
                string searchTerm = txtSearch.Text.Trim();
                List<producto> productosToExport = _productoRepository.GetAll().ToList();

                // *** CAMBIO CLAVE AQUÍ: Obtener el stock de cada producto para la exportación usando el SP ***
                foreach (var prod in productosToExport)
                {
                    try
                    {
                        prod.stock = _productoRepository.ObtenerStockProductoViaSP(prod.id_producto);
                    }
                    catch (Exception exStock)
                    {
                        Log.Escribir($"Error al obtener stock para exportación de producto ID {prod.id_producto} vía SP: {exStock.Message}", exStock);
                        prod.stock = -1; // Indicar un stock desconocido o error
                    }
                }

                // Aplicar filtro para la exportación
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    productosToExport = productosToExport
                        .Where(p => p.nombre.ToLower().Contains(searchTerm.ToLower()) ||
                                     p.descripcion.ToLower().Contains(searchTerm.ToLower()))
                        .ToList();
                }

                // Aplicar ordenación para la exportación
                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_producto": productosToExport = productosToExport.OrderBy(p => p.id_producto).ToList(); break;
                            case "nombre": productosToExport = productosToExport.OrderBy(p => p.nombre).ToList(); break;
                            case "descripcion": productosToExport = productosToExport.OrderBy(p => p.descripcion).ToList(); break;
                            case "precio": productosToExport = productosToExport.OrderBy(p => p.precio).ToList(); break;
                            case "stock": productosToExport = productosToExport.OrderBy(p => p.stock).ToList(); break;
                            default: productosToExport = productosToExport.OrderBy(p => p.id_producto).ToList(); break;
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_producto": productosToExport = productosToExport.OrderByDescending(p => p.id_producto).ToList(); break;
                            case "nombre": productosToExport = productosToExport.OrderByDescending(p => p.nombre).ToList(); break;
                            case "descripcion": productosToExport = productosToExport.OrderByDescending(p => p.descripcion).ToList(); break;
                            case "precio": productosToExport = productosToExport.OrderByDescending(p => p.precio).ToList(); break;
                            case "stock": productosToExport = productosToExport.OrderByDescending(p => p.stock).ToList(); break;
                            default: productosToExport = productosToExport.OrderByDescending(p => p.id_producto).ToList(); break;
                        }
                    }
                }
                else
                {
                    productosToExport = productosToExport.OrderBy(p => p.id_producto).ToList();
                }


                GridView gvExport = new GridView();
                gvExport.DataSource = productosToExport;

                // Definir las columnas para la exportación (solo las visibles y relevantes)
                gvExport.Columns.Add(new BoundField { DataField = "id_producto", HeaderText = "ID" });
                gvExport.Columns.Add(new BoundField { DataField = "nombre", HeaderText = "Nombre" });
                gvExport.Columns.Add(new BoundField { DataField = "descripcion", HeaderText = "Descripción" });
                gvExport.Columns.Add(new BoundField { DataField = "precio", HeaderText = "Precio" });
                gvExport.Columns.Add(new BoundField { DataField = "stock", HeaderText = "Stock" });

                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Productos_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
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
                Log.Escribir($"Error al exportar a Excel: {ex.Message}", ex);
                lblErrorMessage.Text = $"Ocurrió un error al exportar los datos: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        // Método necesario para que RenderControl funcione correctamente al exportar.
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}