using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using ferreteria_je.Repositories.RepositoriesGeneric;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Reflection; // Necesario para PropertyInfo
using System.Globalization; // Necesario para ParseExact

// Importar los nuevos repositorios específicos con su nuevo namespace
using ferreteria_je.Repositories.Inicio;

namespace ferreteria_je
{
    public partial class inicio : ferreteria_je.session.BasePage
    {
        // ====================================================================================================
        // DECLARACIÓN DE INSTANCIAS DE REPOSITORIOS
        // ====================================================================================================

        private GenericRepository<usuarios> _usuariosRepo;
        private GenericRepository<proveedor> _proveedorRepo;
        private GenericRepository<compra> _compraRepo;
        private GenericRepository<cliente> _clienteRepo;
        private GenericRepository<producto> _productoRepo;
        private GenericRepository<venta> _ventaRepo;

        private InicioVentaRepository _inicioVentaRepo;
        private InicioProductoRepository _inicioProductoRepo;

        // ====================================================================================================
        // MODELO Y DATOS SIMULADOS PARA NOTIFICACIONES GENERALES
        // ====================================================================================================
        public class Notification
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public string IconClass { get; set; }
            public bool IsRead { get; set; }
            public DateTime DateCreated { get; set; }
        }

        private static List<Notification> _generalNotifications;

        protected Literal litUserNameButton;
        protected Literal litUserFullName;
        protected Literal litUserEmail;
        protected Literal litUserPhone;


        protected void Page_Load(object sender, EventArgs e)
        {
            // ==============================================
            // VALIDACIÓN DE SESIÓN Y ROL (ADMIN)
            // ==============================================
            if (Session["usuario"] == null || Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
            {
                Response.Redirect("~/vistas/login.aspx");
                return;
            }

            // ==============================================
            // INICIALIZACIÓN DE REPOSITORIOS
            // ==============================================
            _usuariosRepo = new GenericRepository<usuarios>();
            _proveedorRepo = new GenericRepository<proveedor>();
            _compraRepo = new GenericRepository<compra>();
            _clienteRepo = new GenericRepository<cliente>();
            _productoRepo = new GenericRepository<producto>();
            _ventaRepo = new GenericRepository<venta>();

            _inicioVentaRepo = new InicioVentaRepository();
            _inicioProductoRepo = new InicioProductoRepository();

            if (!IsPostBack)
            {
                if (_generalNotifications == null)
                {
                    _generalNotifications = new List<Notification>
            {
                new Notification { Id = 1, Message = "Nuevo pedido #1001 registrado. Revisar detalles.", IconClass = "fas fa-shopping-cart", IsRead = false, DateCreated = DateTime.Now.AddHours(-1) },
                new Notification { Id = 2, Message = "Producto 'Martillo de Uña' tiene stock bajo (5 unidades).", IconClass = "fas fa-exclamation-circle", IsRead = false, DateCreated = DateTime.Now.AddHours(-3) },
                new Notification { Id = 3, Message = "El proveedor 'Herramientas Pro' ha actualizado sus precios.", IconClass = "fas fa-truck-loading", IsRead = true, DateCreated = DateTime.Now.AddDays(-1) },
                new Notification { Id = 4, Message = "Se ha generado el informe de ventas del mes anterior.", IconClass = "fas fa-chart-line", IsRead = true, DateCreated = DateTime.Now.AddDays(-2) }
            };
                }

                CargarDatosUsuarioTopbar();
                LoadDashboardData();
            }
        }


        /// <summary>
        /// Carga los datos del usuario actual en el topbar.
        /// </summary>
        private void CargarDatosUsuarioTopbar()
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
                litUserNameButton.Text = "Invitado";
                litUserFullName.Text = "N/A";
                litUserEmail.Text = "N/A";
                litUserPhone.Text = "N/A";
            }
        }

        /// <summary>
        /// Carga todos los datos para los KPIs y listas del dashboard.
        /// </summary>
        private void LoadDashboardData()
        {
            // Carga de los KPI cards
            CargarCantidadUsuarios();
            CargarCantidadProveedores();
            CargarCantidadCompras();
            CargarCantidadProductos();
            CargarCantidadClientes();
            CargarCantidadVentasMesActual();
            CargarVentasHoy();
            CargarProductosBajoStockKPI();

            // Cargar datos para listas y GridView
            LoadLatestOrders();
            LoadLowStockAlerts();

            // Cargar las tablas detalladas con o sin filtro
            LoadTodosProductos();
            LoadTodasVentas();
            LoadTodosClientes();
            LoadTodasCompras();
            LoadTodosProveedores();
        }

        // --- Métodos para los KPI Cards (existentes) ---
        private void CargarCantidadUsuarios()
        {
            try { lblCantidadUsuarios.Text = _usuariosRepo.GetAll().Count().ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de usuarios en el dashboard", ex); lblCantidadUsuarios.Text = "N/A"; }
        }

        private void CargarCantidadProveedores()
        {
            try { lblCantidadProveedores.Text = _proveedorRepo.GetAll().Count().ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de proveedores en el dashboard", ex); lblCantidadProveedores.Text = "N/A"; }
        }

        private void CargarCantidadCompras()
        {
            try { lblCantidadCompras.Text = _compraRepo.GetAll().Count().ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de compras en el dashboard", ex); lblCantidadCompras.Text = "N/A"; }
        }

        private void CargarCantidadProductos()
        {
            try { lblCantidadProductos.Text = _productoRepo.GetAll().Count().ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de productos en el dashboard", ex); lblCantidadProductos.Text = "N/A"; }
        }

        private void CargarCantidadClientes()
        {
            try { lblCantidadClientes.Text = _clienteRepo.GetAll().Count().ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de clientes en el dashboard", ex); lblCantidadClientes.Text = "N/A"; }
        }

        private void CargarCantidadVentasMesActual()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                string query = "SELECT COUNT(*) FROM venta WHERE MONTH(Fecha) = MONTH(CURDATE()) AND YEAR(Fecha) = YEAR(CURDATE())";
                using (MySqlConnection con = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    con.Open();
                    int cantidadVentas = Convert.ToInt32(cmd.ExecuteScalar());
                    lblCantidadVentas.Text = cantidadVentas.ToString();
                }
            }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de ventas del mes actual", ex); lblCantidadVentas.Text = "N/A"; }
        }

        private void CargarVentasHoy()
        {
            try { decimal ventasHoy = _inicioVentaRepo.GetVentasHoy(); lblVentasHoy.Text = ventasHoy.ToString("C"); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar las ventas de hoy", ex); lblVentasHoy.Text = "N/A"; }
        }

        private void CargarProductosBajoStockKPI()
        {
            try { int umbral = 10; int count = _inicioProductoRepo.GetCantidadProductosBajoStock(umbral); lblProductosBajoStock.Text = count.ToString(); }
            catch (Exception ex) { Utilidades.Log.Escribir("Error al cargar la cantidad de productos bajo stock", ex); lblProductosBajoStock.Text = "N/A"; }
        }

        // --- Métodos para Listas y GridView (existentes) ---
        private void LoadLatestOrders()
        {
            try
            {
                int limit = 5;
                List<InicioVentaRepository.LatestOrderDto> latestOrders = _inicioVentaRepo.GetLatestVentas(limit);

                if (latestOrders.Any())
                {
                    rptLatestOrders.DataSource = latestOrders;
                    rptLatestOrders.DataBind();
                    litNoLatestOrders.Visible = false;
                }
                else
                {
                    rptLatestOrders.DataSource = null;
                    rptLatestOrders.DataBind();
                    litNoLatestOrders.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar los últimos pedidos: " + ex.Message, ex);
                rptLatestOrders.DataSource = null;
                rptLatestOrders.DataBind();
                litNoLatestOrders.Visible = true;
            }
        }

        private void LoadLowStockAlerts()
        {
            try
            {
                int umbral = 10;
                List<producto> lowStockProducts = _inicioProductoRepo.GetProductosBajoStock(umbral);

                if (lowStockProducts.Any())
                {
                    gvLowStockProducts.DataSource = lowStockProducts;
                    gvLowStockProducts.DataBind();
                    gvLowStockProducts.Visible = true;
                }
                else
                {
                    gvLowStockProducts.DataSource = null;
                    gvLowStockProducts.DataBind();
                    gvLowStockProducts.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar alertas de stock bajo: " + ex.Message, ex);
                gvLowStockProducts.DataSource = null;
                gvLowStockProducts.DataBind();
                gvLowStockProducts.Visible = false;
            }
        }

        // --- MÉTODOS PARA LAS TABLAS COMPLETAS (CON FILTRADO) ---

        /// <summary>
        /// Método genérico para filtrar una lista por una propiedad de texto.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto en la lista.</typeparam>
        /// <param name="list">La lista original.</param>
        /// <param name="propertyName">Nombre de la propiedad por la que se filtrará (ej. "nombre").</param>
        /// <param name="filterText">El texto a buscar.</param>
        /// <returns>Lista filtrada.</returns>
        private List<T> FilterListByText<T>(List<T> list, string propertyName, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
            {
                return list;
            }

            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(string))
            {
                // Si la propiedad no existe o no es string, no podemos filtrar por texto
                return list;
            }

            string lowerCaseFilter = filterText.ToLower();
            return list.Where(item =>
            {
                string value = (string)prop.GetValue(item);
                return value != null && value.ToLower().Contains(lowerCaseFilter);
            }).ToList();
        }

        /// <summary>
        /// Método genérico para filtrar una lista por una propiedad de fecha.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto en la lista.</typeparam>
        /// <param name="list">La lista original.</param>
        /// <param name="propertyName">Nombre de la propiedad por la que se filtrará (ej. "fecha").</param>
        /// <param name="filterDate">La fecha exacta a buscar.</param>
        /// <returns>Lista filtrada.</returns>
        private List<T> FilterListByDate<T>(List<T> list, string propertyName, DateTime? filterDate)
        {
            if (!filterDate.HasValue)
            {
                return list;
            }

            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(DateTime))
            {
                return list;
            }

            return list.Where(item =>
            {
                DateTime itemDate = (DateTime)prop.GetValue(item);
                return itemDate.Date == filterDate.Value.Date; // Compara solo la fecha, ignorando la hora
            }).ToList();
        }


        private void LoadTodosProductos()
        {
            try
            {
                var todosProductos = _productoRepo.GetAll().ToList();
                string filterText = txtFilterProductos.Text;
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    todosProductos = FilterListByText(todosProductos, "nombre", filterText);
                }

                if (todosProductos.Any())
                {
                    gvTodosProductos.DataSource = todosProductos;
                    gvTodosProductos.DataBind();
                    gvTodosProductos.Visible = true;
                }
                else
                {
                    gvTodosProductos.DataSource = null;
                    gvTodosProductos.DataBind();
                    gvTodosProductos.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar todos los productos: " + ex.Message, ex);
                gvTodosProductos.DataSource = null;
                gvTodosProductos.DataBind();
                gvTodosProductos.Visible = true;
            }
        }

        protected void btnFilterProductos_Click(object sender, EventArgs e)
        {
            LoadTodosProductos();
            UpdatePanelAllTables.Update(); // Actualiza el UpdatePanel que contiene las tablas
        }

        protected void btnClearFilterProductos_Click(object sender, EventArgs e)
        {
            txtFilterProductos.Text = string.Empty;
            LoadTodosProductos();
            UpdatePanelAllTables.Update();
        }

        private void LoadTodasVentas()
        {
            try
            {
                var todasVentas = _ventaRepo.GetAll().ToList();
                DateTime? filterDate = null;
                if (DateTime.TryParse(txtFilterVentas.Text, out DateTime parsedDate))
                {
                    filterDate = parsedDate;
                }

                if (filterDate.HasValue)
                {
                    todasVentas = FilterListByDate(todasVentas, "fecha", filterDate);
                }

                if (todasVentas.Any())
                {
                    gvTodasVentas.DataSource = todasVentas;
                    gvTodasVentas.DataBind();
                    gvTodasVentas.Visible = true;
                }
                else
                {
                    gvTodasVentas.DataSource = null;
                    gvTodasVentas.DataBind();
                    gvTodasVentas.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar todas las ventas: " + ex.Message, ex);
                gvTodasVentas.DataSource = null;
                gvTodasVentas.DataBind();
                gvTodasVentas.Visible = true;
            }
        }

        protected void btnFilterVentas_Click(object sender, EventArgs e)
        {
            LoadTodasVentas();
            UpdatePanelAllTables.Update();
        }

        protected void btnClearFilterVentas_Click(object sender, EventArgs e)
        {
            txtFilterVentas.Text = string.Empty;
            LoadTodasVentas();
            UpdatePanelAllTables.Update();
        }

        private void LoadTodosClientes()
        {
            try
            {
                var todosClientes = _clienteRepo.GetAll().ToList();
                string filterText = txtFilterClientes.Text;
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    todosClientes = FilterListByText(todosClientes, "nombre", filterText);
                }

                if (todosClientes.Any())
                {
                    gvTodosClientes.DataSource = todosClientes;
                    gvTodosClientes.DataBind();
                    gvTodosClientes.Visible = true;
                }
                else
                {
                    gvTodosClientes.DataSource = null;
                    gvTodosClientes.DataBind();
                    gvTodosClientes.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar todos los clientes: " + ex.Message, ex);
                gvTodosClientes.DataSource = null;
                gvTodosClientes.DataBind();
                gvTodosClientes.Visible = true;
            }
        }

        protected void btnFilterClientes_Click(object sender, EventArgs e)
        {
            LoadTodosClientes();
            UpdatePanelAllTables.Update();
        }

        protected void btnClearFilterClientes_Click(object sender, EventArgs e)
        {
            txtFilterClientes.Text = string.Empty;
            LoadTodosClientes();
            UpdatePanelAllTables.Update();
        }

        private void LoadTodasCompras()
        {
            try
            {
                var todasCompras = _compraRepo.GetAll().ToList();
                DateTime? filterDate = null;
                if (DateTime.TryParse(txtFilterCompras.Text, out DateTime parsedDate))
                {
                    filterDate = parsedDate;
                }

                if (filterDate.HasValue)
                {
                    todasCompras = FilterListByDate(todasCompras, "fecha", filterDate);
                }

                if (todasCompras.Any())
                {
                    gvTodasCompras.DataSource = todasCompras;
                    gvTodasCompras.DataBind();
                    gvTodasCompras.Visible = true;
                }
                else
                {
                    gvTodasCompras.DataSource = null;
                    gvTodasCompras.DataBind();
                    gvTodasCompras.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar todas las compras: " + ex.Message, ex);
                gvTodasCompras.DataSource = null;
                gvTodasCompras.DataBind();
                gvTodasCompras.Visible = true;
            }
        }

        protected void btnFilterCompras_Click(object sender, EventArgs e)
        {
            LoadTodasCompras();
            UpdatePanelAllTables.Update();
        }

        protected void btnClearFilterCompras_Click(object sender, EventArgs e)
        {
            txtFilterCompras.Text = string.Empty;
            LoadTodasCompras();
            UpdatePanelAllTables.Update();
        }

        private void LoadTodosProveedores()
        {
            try
            {
                var todosProveedores = _proveedorRepo.GetAll().ToList();
                string filterText = txtFilterProveedores.Text;
                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    todosProveedores = FilterListByText(todosProveedores, "nombre", filterText);
                }

                if (todosProveedores.Any())
                {
                    gvTodosProveedores.DataSource = todosProveedores;
                    gvTodosProveedores.DataBind();
                    gvTodosProveedores.Visible = true;
                }
                else
                {
                    gvTodosProveedores.DataSource = null;
                    gvTodosProveedores.DataBind();
                    gvTodosProveedores.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al cargar todos los proveedores: " + ex.Message, ex);
                gvTodosProveedores.DataSource = null;
                gvTodosProveedores.DataBind();
                gvTodosProveedores.Visible = true;
            }
        }

        protected void btnFilterProveedores_Click(object sender, EventArgs e)
        {
            LoadTodosProveedores();
            UpdatePanelAllTables.Update();
        }

        protected void btnClearProveedores_Click(object sender, EventArgs e)
        {
            txtFilterProveedores.Text = string.Empty;
            LoadTodosProveedores();
            UpdatePanelAllTables.Update();
        }


        // --- LÓGICA PARA EXPORTAR A EXCEL (CORREGIDA) ---

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = "ReporteDashboardFerreteria_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                EnableViewState = false; // Deshabilitar ViewState para una exportación limpia

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        // Exportar gvLowStockProducts (Alertas de Stock Bajo)
                        if (gvLowStockProducts.Visible && gvLowStockProducts.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Alertas de Stock Bajo</h2>");
                            // Para exportar, el GridView debe estar fuera de un UpdatePanel o RenderControl debe ser llamado en Page_Load sin IsPostBack
                            // La forma más robusta para exportar dentro de un UpdatePanel es renderizar el control en memoria si está en un UP.
                            // Pero dado que ya tienes VerifyRenderingInServerForm, el problema podría ser el ViewState.
                            gvLowStockProducts.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        // Exportar gvTodosProductos (Todos los Productos)
                        if (gvTodosProductos.Visible && gvTodosProductos.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Todos los Productos</h2>");
                            gvTodosProductos.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        // Exportar gvTodasVentas (Todas las Ventas)
                        if (gvTodasVentas.Visible && gvTodasVentas.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Todas las Ventas</h2>");
                            gvTodasVentas.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        // Exportar gvTodosClientes (Todos los Clientes)
                        if (gvTodosClientes.Visible && gvTodosClientes.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Todos los Clientes</h2>");
                            gvTodosClientes.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        // Exportar gvTodasCompras (Todas las Compras)
                        if (gvTodasCompras.Visible && gvTodasCompras.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Todas las Compras</h2>");
                            gvTodasCompras.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        // Exportar gvTodosProveedores (Todos los Proveedores)
                        if (gvTodosProveedores.Visible && gvTodosProveedores.Rows.Count > 0)
                        {
                            hw.WriteLine("<h2>Todos los Proveedores</h2>");
                            gvTodosProveedores.RenderControl(hw);
                            hw.WriteLine("<br/>");
                        }

                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Esto es esperado cuando se usa Response.End()
            }
            catch (Exception ex)
            {
                Utilidades.Log.Escribir("Error al exportar a Excel: " + ex.Message, ex);
                // Puedes mostrar un mensaje al usuario si el error es crítico
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error al exportar a Excel: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        // Este método es necesario para que RenderControl funcione con un GridView dentro de un Form
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Hace que el control se renderice en el formulario */
        }


        // --- Gestión de Sesión ---
        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("../vistas/login.aspx");
        }
    }
}