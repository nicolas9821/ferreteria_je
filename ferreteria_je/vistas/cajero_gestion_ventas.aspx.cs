using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using ferreteria_je.Repositories.RepositoriesGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Dapper.Contrib.Extensions; // Necessary for [Table] and [Key] if you use them in Venta model

namespace ferreteria_je
{
    public partial class cajero_gestion_ventas : ferreteria_je.session.BasePage
    {
        // Declare generic repository instances for each entity using IRepository interface
        private IRepository<venta> _ventaRepository;
        private IRepository<cliente> _clienteRepository;
        private IRepository<usuario> _usuarioRepository;
        private IRepository<producto> _productoRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize generic repositories with the specific entity type
            _ventaRepository = new GenericRepository<venta>();
            _clienteRepository = new GenericRepository<cliente>();
            _usuarioRepository = new GenericRepository<usuario>();
            _productoRepository = new GenericRepository<producto>();

            if (!IsPostBack)
            {
                PopulateDropDownLists(); // Populate dropdowns on initial load

                // Check for Venta ID in query string for edit mode
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int idToLoad))
                    {
                        hdnVentaId.Value = idToLoad.ToString(); // Store ID in hidden field
                        CargarDatosVenta(idToLoad);
                        btnActualizar.Visible = true;
                        btnAgregar.Visible = false;
                    }
                    else
                    {
                        MostrarMensaje("ID de venta inválido en la URL. Ingresando en modo 'Registrar'.", "warning");
                        LimpiarCampos();
                        btnActualizar.Visible = true;
                        btnAgregar.Visible = true;
                    }
                }
                else
                {
                    // No ID in URL, default to Add mode
                    LimpiarCampos();
                    btnActualizar.Visible = true;
                    btnAgregar.Visible = true;
                }
            }
        }

        private void PopulateDropDownLists()
        {
            try
            {
                // Populate Clientes
                ddlCliente.DataSource = _clienteRepository.GetAll();
                ddlCliente.DataTextField = "nombre";
                ddlCliente.DataValueField = "id_cliente";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("-- Seleccione Cliente --", ""));

                // Populate Usuarios
                ddlUsuario.DataSource = _usuarioRepository.GetAll();
                ddlUsuario.DataTextField = "nombre";
                ddlUsuario.DataValueField = "id_usuario";
                ddlUsuario.DataBind();
                ddlUsuario.Items.Insert(0, new ListItem("-- Seleccione Usuario --", ""));

                // Populate Productos
                ddlProducto.DataSource = _productoRepository.GetAll();
                ddlProducto.DataTextField = "nombre";
                ddlProducto.DataValueField = "id_producto";
                ddlProducto.DataBind();
                ddlProducto.Items.Insert(0, new ListItem("-- Seleccione Producto --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar listas desplegables: {ex.Message}", "error");
            }
        }

        private void CargarDatosVenta(int idVenta)
        {
            try
            {
                var ventaExistente = _ventaRepository.Get(v => v.id_venta == idVenta);

                if (ventaExistente != null)
                {
                    txtIdVenta.Text = ventaExistente.id_venta.ToString();
                    txtFecha.Text = ventaExistente.fecha.ToString("yyyy-MM-dd"); // Use 'fecha' from model

                    // Handle nullable int? properties using null-coalescing operator (?? 0)
                    if (ventaExistente.id_cliente.HasValue && ddlCliente.Items.FindByValue(ventaExistente.id_cliente.Value.ToString()) != null)
                        ddlCliente.SelectedValue = ventaExistente.id_cliente.Value.ToString();
                    else if (!ventaExistente.id_cliente.HasValue && ddlCliente.Items.Count > 0) // Select default if null
                        ddlCliente.SelectedIndex = 0;


                    if (ventaExistente.id_usuario.HasValue && ddlUsuario.Items.FindByValue(ventaExistente.id_usuario.Value.ToString()) != null)
                        ddlUsuario.SelectedValue = ventaExistente.id_usuario.Value.ToString();
                    else if (!ventaExistente.id_usuario.HasValue && ddlUsuario.Items.Count > 0) // Select default if null
                        ddlUsuario.SelectedIndex = 0;


                    if (ventaExistente.id_producto.HasValue && ddlProducto.Items.FindByValue(ventaExistente.id_producto.Value.ToString()) != null)
                        ddlProducto.SelectedValue = ventaExistente.id_producto.Value.ToString();
                    else if (!ventaExistente.id_producto.HasValue && ddlProducto.Items.Count > 0) // Select default if null
                        ddlProducto.SelectedIndex = 0;


                    txtCantidad.Text = ventaExistente.cantidad.ToString();
                    txtPrecioUnitario.Text = ventaExistente.precio_unitario.ToString("F2");
                    txtTotalVenta.Text = ventaExistente.total.ToString("F2"); // Use 'total' from model

                    MostrarMensaje($"Venta ID {idVenta} cargada exitosamente para edición.", "info");
                }
                else
                {
                    MostrarMensaje($"No se encontró ninguna venta con el ID: {idVenta}. Volviendo a modo 'Registrar'.", "warning");
                    LimpiarCampos();
                    hdnVentaId.Value = "";
                    btnActualizar.Visible = true;
                    btnAgregar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la venta: {ex.Message}", "error");
                LimpiarCampos();
                hdnVentaId.Value = "";
                btnActualizar.Visible = true;
                btnAgregar.Visible = true;
            }
        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(ddlProducto.SelectedValue, out int selectedProductId))
            {
                var producto = _productoRepository.Get(p => p.id_producto == selectedProductId);
                if (producto != null)
                {
                    txtPrecioUnitario.Text = producto.precio.ToString("F2");
                    CalcularTotalVenta();
                }
                else
                {
                    txtPrecioUnitario.Text = "0.00";
                    MostrarMensaje("Producto no encontrado o seleccionado inválido.", "warning");
                    txtTotalVenta.Text = "0.00";
                }
            }
            else
            {
                txtPrecioUnitario.Text = "0.00";
                txtTotalVenta.Text = "0.00";
            }
        }

        protected void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            CalcularTotalVenta();
        }

        private void CalcularTotalVenta()
        {
            decimal precioUnitario = 0;
            int cantidad = 0;

            if (decimal.TryParse(txtPrecioUnitario.Text, out precioUnitario) &&
                int.TryParse(txtCantidad.Text, out cantidad))
            {
                if (cantidad < 0)
                {
                    MostrarMensaje("La cantidad no puede ser negativa.", "warning");
                    txtTotalVenta.Text = "0.00";
                    return;
                }
                if (precioUnitario < 0)
                {
                    MostrarMensaje("El precio unitario no puede ser negativo.", "warning");
                    txtTotalVenta.Text = "0.00";
                    return;
                }

                txtTotalVenta.Text = (precioUnitario * cantidad).ToString("F2");
                lblMensaje.Text = string.Empty;
            }
            else
            {
                txtTotalVenta.Text = "0.00";
                if (!string.IsNullOrWhiteSpace(txtPrecioUnitario.Text) && !decimal.TryParse(txtPrecioUnitario.Text, out _))
                {
                    MostrarMensaje("Formato de precio unitario inválido.", "warning");
                }
                if (!string.IsNullOrWhiteSpace(txtCantidad.Text) && !int.TryParse(txtCantidad.Text, out _))
                {
                    MostrarMensaje("Formato de cantidad inválido.", "warning");
                }
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            hdnVentaId.Value = "";

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                venta nuevaVenta = new venta
                {
                    fecha = DateTime.Parse(txtFecha.Text.Trim()), // Use 'fecha'
                    // For nullable FKs, assign directly from ddl.SelectedValue
                    // If your DB allows NULL for FK, you can set to null or handle accordingly
                    id_cliente = int.Parse(ddlCliente.SelectedValue), // SelectedValue is string, will be null or empty if "-- Seleccione --" is chosen
                    id_usuario = int.Parse(ddlUsuario.SelectedValue),
                    id_producto = int.Parse(ddlProducto.SelectedValue),
                    cantidad = int.Parse(txtCantidad.Text.Trim()),
                    precio_unitario = decimal.Parse(txtPrecioUnitario.Text.Trim()),
                    total = decimal.Parse(txtTotalVenta.Text.Trim()) // Use 'total'
                };

                var selectedProduct = _productoRepository.Get(p => p.id_producto == (nuevaVenta.id_producto ?? 0)); // Use ?? 0 for safety
                if (selectedProduct == null || selectedProduct.stock < nuevaVenta.cantidad)
                {
                    MostrarMensaje("Stock insuficiente para el producto seleccionado.", "warning");
                    return;
                }

                _ventaRepository.Add(nuevaVenta);

                selectedProduct.stock -= nuevaVenta.cantidad;
                _productoRepository.Update(selectedProduct);

                MostrarMensaje("Venta registrada exitosamente!", "success");
                LimpiarCampos();
            }
            catch (FormatException)
            {
                MostrarMensaje("Error de formato: Asegúrese de que todos los campos numéricos y de fecha sean válidos.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar la venta: " + ex.Message, "error");
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hdnVentaId.Value) || !int.TryParse(hdnVentaId.Value, out int idVentaToUpdate))
            {
                MostrarMensaje("No hay una venta seleccionada para actualizar. Por favor, cargue una venta o registre una nueva.", "warning");
                return;
            }

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                var ventaToUpdate = _ventaRepository.Get(v => v.id_venta == idVentaToUpdate);

                if (ventaToUpdate == null)
                {
                    MostrarMensaje("La venta que intenta actualizar ya no existe. Limpiando formulario.", "error");
                    LimpiarCampos();
                    hdnVentaId.Value = "";
                    btnAgregar.Visible = true;
                    btnActualizar.Visible = true;
                    return;
                }

                // Store original quantity and product ID for stock adjustment
                int originalQuantity = ventaToUpdate.cantidad;
                int originalProductId = ventaToUpdate.id_producto ?? 0; // Use ?? 0 as id_producto is int?

                ventaToUpdate.fecha = DateTime.Parse(txtFecha.Text.Trim()); // Use 'fecha'
                ventaToUpdate.id_cliente = int.Parse(ddlCliente.SelectedValue);
                ventaToUpdate.id_usuario = int.Parse(ddlUsuario.SelectedValue);
                ventaToUpdate.id_producto = int.Parse(ddlProducto.SelectedValue);
                ventaToUpdate.cantidad = int.Parse(txtCantidad.Text.Trim());
                ventaToUpdate.precio_unitario = decimal.Parse(txtPrecioUnitario.Text.Trim());
                ventaToUpdate.total = decimal.Parse(txtTotalVenta.Text.Trim()); // Use 'total'

                // Handle stock adjustment logic
                if (originalProductId != (ventaToUpdate.id_producto ?? 0) || originalQuantity != ventaToUpdate.cantidad)
                {
                    // Restore stock for original product (if product changed or quantity decreased)
                    var oldProduct = _productoRepository.Get(p => p.id_producto == originalProductId);
                    if (oldProduct != null)
                    {
                        oldProduct.stock += originalQuantity;
                        _productoRepository.Update(oldProduct);
                    }

                    // Deduct stock for new product/quantity
                    var newProduct = _productoRepository.Get(p => p.id_producto == (ventaToUpdate.id_producto ?? 0)); // Use ?? 0 for safety
                    if (newProduct != null)
                    {
                        // Calculate stock difference for the same product
                        if (originalProductId == (ventaToUpdate.id_producto ?? 0)) // Product is the same
                        {
                            int stockChange = originalQuantity - ventaToUpdate.cantidad; // Positive if quantity decreased, negative if increased
                            newProduct.stock += stockChange; // Add difference back to stock
                            if (newProduct.stock < 0) // Check for negative stock after adjustment
                            {
                                MostrarMensaje("Stock insuficiente para la nueva cantidad del producto.", "warning");
                                // Revert stock change
                                newProduct.stock -= stockChange;
                                _productoRepository.Update(newProduct); // Save reverted stock
                                return;
                            }
                        }
                        else // Product changed
                        {
                            if (newProduct.stock < ventaToUpdate.cantidad)
                            {
                                MostrarMensaje("Stock insuficiente para el nuevo producto seleccionado.", "warning");
                                // IMPORTANT: Revert previous stock changes if this fails
                                if (oldProduct != null) { oldProduct.stock -= originalQuantity; _productoRepository.Update(oldProduct); }
                                return;
                            }
                            newProduct.stock -= ventaToUpdate.cantidad;
                        }
                        _productoRepository.Update(newProduct);
                    }
                }

                _ventaRepository.Update(ventaToUpdate);

                MostrarMensaje("¡Venta actualizada exitosamente!", "success");
                LimpiarCampos();
                hdnVentaId.Value = "";
                btnAgregar.Visible = true;
                btnActualizar.Visible = true;
            }
            catch (FormatException)
            {
                MostrarMensaje("Error de formato: Asegúrese de que todos los campos numéricos y de fecha sean válidos.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al actualizar la venta: " + ex.Message, "error");
            }
        }


        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MostrarMensaje("Campos limpiados.", "info");
            hdnVentaId.Value = "";
            btnAgregar.Visible = true;
            btnActualizar.Visible = true;
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("cajero_ventas.aspx");
        }

        private void LimpiarCampos()
        {
            txtIdVenta.Text = "Automático";
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd"); // Set to current date
            if (ddlCliente.Items.Count > 0) ddlCliente.SelectedIndex = 0;
            if (ddlUsuario.Items.Count > 0) ddlUsuario.SelectedIndex = 0;
            if (ddlProducto.Items.Count > 0) ddlProducto.SelectedIndex = 0;
            txtCantidad.Text = string.Empty;
            txtPrecioUnitario.Text = "0.00";
            txtTotalVenta.Text = "0.00";
            lblMensaje.Text = string.Empty;
            lblMensaje.CssClass = "";
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtFecha.Text) ||
                ddlCliente.SelectedValue == "" ||
                ddlUsuario.SelectedValue == "" ||
                ddlProducto.SelectedValue == "" ||
                string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MostrarMensaje("Por favor, complete todos los campos obligatorios.", "warning");
                return false;
            }

            if (!DateTime.TryParse(txtFecha.Text, out _))
            {
                MostrarMensaje("Formato de fecha inválido. Use AAAA-MM-DD.", "warning");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("La cantidad debe ser un número entero positivo.", "warning");
                return false;
            }

            if (!decimal.TryParse(txtPrecioUnitario.Text, out decimal precioUnitario) || precioUnitario < 0)
            {
                MostrarMensaje("El precio unitario debe ser un número válido (no negativo).", "warning");
                return false;
            }

            if (!decimal.TryParse(txtTotalVenta.Text, out decimal totalVenta) || totalVenta < 0)
            {
                MostrarMensaje("El total de venta debe ser un número válido (no negativo).", "warning");
                return false;
            }

            return true;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = "mensaje";
            switch (tipo)
            {
                case "success":
                    lblMensaje.CssClass += " success";
                    break;
                case "error":
                    lblMensaje.CssClass += " error";
                    break;
                case "warning":
                    lblMensaje.CssClass += " alert-warning";
                    break;
                case "info":
                    lblMensaje.CssClass += " alert-info";
                    break;
                default:
                    lblMensaje.CssClass += " alert-info";
                    break;
            }
        }
    }
}