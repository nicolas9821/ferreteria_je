using System;
using System.Configuration;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;

namespace ferreteria_je
{
    public partial class gestionventas : ferreteria_je.session.BasePage
    {
        // Instancias de los repositorios
        private VentaRepository _ventaRepository;
        private ProductoRepository _productoRepository;
        private ClienteRepository _clienteRepository;
        private UsuarioRepository _usuarioRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Siempre inicializa los repositorios en Page_Load
            _ventaRepository = new VentaRepository();
            _productoRepository = new ProductoRepository();
            _clienteRepository = new ClienteRepository();
            _usuarioRepository = new UsuarioRepository();

            if (!IsPostBack)
            {
                // Cargar DropDownLists al cargar la página por primera vez
                CargarClientes();
                CargarUsuarios();
                CargarProductos();

                // Verificar si se pasó un ID en la URL (modo "Editar" o "Ver Detalle")
                if (Request.QueryString["id"] != null)
                {
                    int idVenta;
                    if (int.TryParse(Request.QueryString["id"], out idVenta))
                    {
                        CargarDatosVenta(idVenta); // Cargar datos de la venta para edición
                        ConfigurarModoEdicion();   // Ajustar visibilidad de botones
                    }
                    else
                    {
                        lblMensaje.Text = "ID de venta inválido en la URL.";
                        ConfigurarModoAgregar();
                    }
                }
                else
                {
                    // No hay ID en la URL, asume modo "Agregar Nuevo"
                    ConfigurarModoAgregar();
                }
            }
        }

        /// <summary>
        /// Configura botones y campos para el modo "Agregar Nueva Venta".
        /// </summary>
        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            txtIdVenta.ReadOnly = true;
            LimpiarCampos();
            lblMensaje.Text = "Ingrese los datos para una nueva venta.";
        }

        /// <summary>
        /// Configura botones y campos para el modo "Editar/Eliminar Venta".
        /// </summary>
        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            txtIdVenta.ReadOnly = true;
        }

        /// <summary>
        /// Carga los clientes en el DropDownList.
        /// </summary>
        private void CargarClientes()
        {
            try
            {
                var clientes = _clienteRepository.GetAll().ToList();
                ddlCliente.DataSource = clientes;
                // CAMBIO AQUÍ: Usamos "nombre" de tu modelo cliente.cs
                ddlCliente.DataTextField = "nombre";
                ddlCliente.DataValueField = "id_cliente";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("-- Seleccione Cliente --", "")); // Opción por defecto
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar clientes: " + ex.Message, ex);
                lblMensaje.Text = "Error al cargar clientes.";
            }
        }

        /// <summary>
        /// Carga los usuarios en el DropDownList.
        /// </summary>
        private void CargarUsuarios()
        {
            try
            {
                var usuarios = _usuarioRepository.GetAll().ToList();
                ddlUsuario.DataSource = usuarios;
                // CAMBIO AQUÍ: Usamos "nombre" de tu modelo usuario.cs
                ddlUsuario.DataTextField = "nombre";
                ddlUsuario.DataValueField = "id_usuario";
                ddlUsuario.DataBind();
                ddlUsuario.Items.Insert(0, new ListItem("-- Seleccione Usuario --", "")); // Opción por defecto
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar usuarios: " + ex.Message, ex);
                lblMensaje.Text = "Error al cargar usuarios.";
            }
        }

        /// <summary>
        /// Carga los productos en el DropDownList.
        /// </summary>
        private void CargarProductos()
        {
            try
            {
                var productos = _productoRepository.GetAll().ToList();
                ddlProducto.DataSource = productos;
                ddlProducto.DataTextField = "nombre"; // Ya estaba correcto
                ddlProducto.DataValueField = "id_producto";
                ddlProducto.DataBind();
                ddlProducto.Items.Insert(0, new ListItem("-- Seleccione Producto --", "")); // Opción por defecto
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar productos: " + ex.Message, ex);
                lblMensaje.Text = "Error al cargar productos.";
            }
        }

        /// <summary>
        /// Carga los datos de una venta específica en los campos del formulario.
        /// </summary>
        /// <param name="idVenta">El ID de la venta a cargar.</param>
        private void CargarDatosVenta(int idVenta)
        {
            try
            {
                var ventaEncontrada = _ventaRepository.GetVentaById(idVenta);

                if (ventaEncontrada != null)
                {
                    txtIdVenta.Text = ventaEncontrada.id_venta.ToString();
                    txtFecha.Text = ventaEncontrada.fecha.ToString("yyyy-MM-dd");
                    txtPrecioUnitario.Text = ventaEncontrada.precio_unitario.ToString("F2");
                    txtCantidad.Text = ventaEncontrada.cantidad.ToString();
                    txtTotalVenta.Text = ventaEncontrada.total.ToString("F2");

                    // Seleccionar los valores en los DropDownLists
                    if (ventaEncontrada.id_cliente.HasValue && ddlCliente.Items.FindByValue(ventaEncontrada.id_cliente.Value.ToString()) != null)
                    {
                        ddlCliente.SelectedValue = ventaEncontrada.id_cliente.Value.ToString();
                    }
                    else
                    {
                        ddlCliente.ClearSelection();
                    }

                    if (ventaEncontrada.id_usuario.HasValue && ddlUsuario.Items.FindByValue(ventaEncontrada.id_usuario.Value.ToString()) != null)
                    {
                        ddlUsuario.SelectedValue = ventaEncontrada.id_usuario.Value.ToString();
                    }
                    else
                    {
                        ddlUsuario.ClearSelection();
                    }

                    if (ventaEncontrada.id_producto.HasValue && ddlProducto.Items.FindByValue(ventaEncontrada.id_producto.Value.ToString()) != null)
                    {
                        ddlProducto.SelectedValue = ventaEncontrada.id_producto.Value.ToString();
                    }
                    else
                    {
                        ddlProducto.ClearSelection();
                    }

                    lblMensaje.Text = "Venta encontrada. Puede modificarla y hacer clic en Actualizar.";
                }
                else
                {
                    lblMensaje.Text = $"Venta con ID {idVenta} no encontrada. Creando una nueva.";
                    ConfigurarModoAgregar();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar datos de la venta con ID {idVenta}: {ex.Message}", ex);
                lblMensaje.Text = "Error al cargar la venta. Verifique los logs para más detalles.";
                ConfigurarModoAgregar();
            }
        }

        /// <summary>
        /// Evento que se dispara cuando cambia la selección del producto.
        /// Recalcula el precio unitario y el total.
        /// </summary>
        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotalVenta();
        }

        /// <summary>
        /// Evento que se dispara cuando cambia la cantidad.
        /// Recalcula el total.
        /// </summary>
        protected void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            CalcularTotalVenta();
        }

        /// <summary>
        /// Calcula el precio unitario (si se selecciona un producto) y el total de la venta.
        /// </summary>
        private void CalcularTotalVenta()
        {
            decimal precioUnitario = 0;
            int cantidad = 0;

            // Obtener el precio unitario del producto seleccionado
            if (!string.IsNullOrEmpty(ddlProducto.SelectedValue))
            {
                if (int.TryParse(ddlProducto.SelectedValue, out int idProducto))
                {
                    var producto = _productoRepository.Get(p => p.id_producto == idProducto);
                    if (producto != null)
                    {
                        // CORRECCIÓN FINAL: Usa 'producto.precio'
                        precioUnitario = producto.precio;
                        txtPrecioUnitario.Text = precioUnitario.ToString("F2");
                    }
                    else
                    {
                        txtPrecioUnitario.Text = string.Empty;
                        lblMensaje.Text = "No se pudo obtener el precio del producto seleccionado.";
                    }
                }
            }
            else
            {
                txtPrecioUnitario.Text = string.Empty;
            }

            // Obtener la cantidad
            if (int.TryParse(txtCantidad.Text, out cantidad))
            {
                txtTotalVenta.Text = (precioUnitario * cantidad).ToString("F2");
            }
            else
            {
                txtTotalVenta.Text = string.Empty;
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validaciones iniciales para campos obligatorios
            if (string.IsNullOrWhiteSpace(txtFecha.Text) ||
                string.IsNullOrWhiteSpace(txtCantidad.Text) ||
                string.IsNullOrWhiteSpace(txtPrecioUnitario.Text) ||
                string.IsNullOrWhiteSpace(txtTotalVenta.Text))
            {
                lblMensaje.Text = "Por favor, complete la fecha, cantidad, precio unitario y total.";
                return;
            }

            // Validaciones para DropDownLists (si son obligatorios en la BD)
            if (string.IsNullOrEmpty(ddlCliente.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un cliente.";
                return;
            }
            if (string.IsNullOrEmpty(ddlUsuario.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un usuario.";
                return;
            }
            if (string.IsNullOrEmpty(ddlProducto.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un producto.";
                return;
            }

            try
            {
                var nuevaVenta = new venta
                {
                    fecha = DateTime.Parse(txtFecha.Text),
                    precio_unitario = decimal.Parse(txtPrecioUnitario.Text),
                    cantidad = int.Parse(txtCantidad.Text),
                    total = decimal.Parse(txtTotalVenta.Text)
                };

                // Asignar ID de cliente, usuario y producto (ya validados, así que parseamos directamente)
                nuevaVenta.id_cliente = int.Parse(ddlCliente.SelectedValue);
                nuevaVenta.id_usuario = int.Parse(ddlUsuario.SelectedValue);
                nuevaVenta.id_producto = int.Parse(ddlProducto.SelectedValue);

                _ventaRepository.Add(nuevaVenta);
                lblMensaje.Text = "Venta agregada correctamente.";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
            catch (Exception ex)
            {
                // *** CAMBIO CLAVE AQUÍ PARA DEPURACIÓN ***
                // Muestra el mensaje de la excepción directamente en la interfaz.
                // NO LO DEJES ASÍ EN PRODUCCIÓN FINAL.
                Log.Escribir("Error al agregar venta: " + ex.Message, ex);
                lblMensaje.Text = "Error al agregar venta. DETALLE: " + ex.Message;
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdVenta.Text) ||
                string.IsNullOrWhiteSpace(txtFecha.Text) ||
                string.IsNullOrWhiteSpace(txtCantidad.Text) ||
                string.IsNullOrWhiteSpace(txtPrecioUnitario.Text) ||
                string.IsNullOrWhiteSpace(txtTotalVenta.Text))
            {
                lblMensaje.Text = "Para actualizar, ingrese un ID, complete la fecha, cantidad, precio unitario y total.";
                return;
            }

            // Validaciones para DropDownLists (si son obligatorios en la BD)
            if (string.IsNullOrEmpty(ddlCliente.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un cliente.";
                return;
            }
            if (string.IsNullOrEmpty(ddlUsuario.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un usuario.";
                return;
            }
            if (string.IsNullOrEmpty(ddlProducto.SelectedValue))
            {
                lblMensaje.Text = "Por favor, seleccione un producto.";
                return;
            }


            int idVenta;
            if (!int.TryParse(txtIdVenta.Text, out idVenta))
            {
                lblMensaje.Text = "ID de venta inválido.";
                return;
            }

            try
            {
                var ventaModificada = new venta
                {
                    id_venta = idVenta,
                    fecha = DateTime.Parse(txtFecha.Text),
                    precio_unitario = decimal.Parse(txtPrecioUnitario.Text),
                    cantidad = int.Parse(txtCantidad.Text),
                    total = decimal.Parse(txtTotalVenta.Text)
                };

                // Asignar ID de cliente, usuario y producto (ya validados, así que parseamos directamente)
                ventaModificada.id_cliente = int.Parse(ddlCliente.SelectedValue);
                ventaModificada.id_usuario = int.Parse(ddlUsuario.SelectedValue);
                ventaModificada.id_producto = int.Parse(ddlProducto.SelectedValue);


                _ventaRepository.Update(ventaModificada);
                lblMensaje.Text = "Venta actualizada correctamente.";
            }
            catch (Exception ex)
            {
                // *** CAMBIO CLAVE AQUÍ PARA DEPURACIÓN ***
                // Muestra el mensaje de la excepción directamente en la interfaz.
                // NO LO DEJES ASÍ EN PRODUCCIÓN FINAL.
                Log.Escribir($"Error al actualizar venta con ID {idVenta}: {ex.Message}", ex);
                lblMensaje.Text = $"Error al actualizar venta con ID {idVenta}. DETALLE: " + ex.Message;
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdVenta.Text))
            {
                lblMensaje.Text = "Ingrese el ID de la venta a eliminar.";
                return;
            }

            int idVenta;
            if (!int.TryParse(txtIdVenta.Text, out idVenta))
            {
                lblMensaje.Text = "ID de venta inválido para eliminar.";
                return;
            }

            try
            {
                var ventaAEliminar = new venta { id_venta = idVenta };

                _ventaRepository.Delete(ventaAEliminar);
                lblMensaje.Text = "Venta eliminada correctamente.";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
            catch (Exception ex)
            {
                // *** CAMBIO CLAVE AQUÍ PARA DEPURACIÓN ***
                // Muestra el mensaje de la excepción directamente en la interfaz.
                // NO LO DEJES ASÍ EN PRODUCCIÓN FINAL.
                Log.Escribir($"Error al eliminar venta con ID {idVenta}: {ex.Message}", ex);
                lblMensaje.Text = $"Error al eliminar venta con ID {idVenta}. DETALLE: " + ex.Message;
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarModoAgregar();
        }

        /// <summary>
        /// Limpia todos los campos de texto del formulario y la etiqueta de mensaje.
        /// </summary>
        private void LimpiarCampos()
        {
            txtIdVenta.Text = string.Empty;
            txtFecha.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtCantidad.Text = string.Empty;
            txtTotalVenta.Text = string.Empty;
            ddlCliente.ClearSelection();
            ddlUsuario.ClearSelection();
            ddlProducto.ClearSelection();
            lblMensaje.Text = string.Empty;
        }
    }
}