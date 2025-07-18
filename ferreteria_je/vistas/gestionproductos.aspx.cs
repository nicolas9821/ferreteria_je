using MySql.Data.MySqlClient; // For specific MySQL exceptions
using System;
using System.Configuration;
using ferreteria_je.Repositories; // Contains ProductoRepository
using ferreteria_je.Repositories.Models; // Contains 'producto' model
using ferreteria_je.Utilidades; // For the Log utility
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ferreteria_je
{
    public partial class gestionproductos : ferreteria_je.session.BasePage
    {
        private ProductoRepository _productoRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            _productoRepository = new ProductoRepository();

            if (!IsPostBack)
            {
                // Check if an 'id' is passed in the URL query string (e.g., gestionproductos.aspx?id=123)
                if (Request.QueryString["id"] != null)
                {
                    int idProducto;
                    if (int.TryParse(Request.QueryString["id"], out idProducto))
                    {
                        CargarDatosProducto(idProducto); // Load product data if ID is valid
                        ConfigurarModoEdicion();         // Set UI for editing/deleting
                    }
                    else
                    {
                        lblMensaje.Text = "ID de producto inválido en la URL.";
                        lblMensaje.CssClass = "mensaje error";
                        ConfigurarModoAgregar();
                    }
                }
                else
                {
                    ConfigurarModoAgregar(); // If no ID, assume adding a new product
                }
            }
        }

        // --- UI Configuration Methods ---

        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            txtIdProducto.ReadOnly = true; // ID should always be read-only in this context
            LimpiarCampos();
            lblMensaje.Text = "Ingrese los datos para un nuevo producto.";
            lblMensaje.CssClass = "mensaje info";
        }

        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            txtIdProducto.ReadOnly = true; // ID should always be read-only in this context
            lblMensaje.Text = "Datos de producto cargados. Puede modificar y Actualizar o Eliminar.";
            lblMensaje.CssClass = "mensaje info";
        }

        // --- Data Loading Method ---

        private void CargarDatosProducto(int idProducto)
        {
            try
            {
                // Use the generic Get method with a predicate to load product details
                var productoEncontrado = _productoRepository.Get(p => p.id_producto == idProducto);

                if (productoEncontrado != null)
                {
                    txtIdProducto.Text = productoEncontrado.id_producto.ToString();
                    txtNombre.Text = productoEncontrado.nombre;
                    txtDescripcion.Text = productoEncontrado.descripcion;
                    txtPrecio.Text = productoEncontrado.precio.ToString();

                    // Obtener el stock usando el SP ObtenerStockProducto (parámetro OUT)
                    try
                    {
                        int stockActual = _productoRepository.ObtenerStockProductoViaSP(idProducto);
                        txtStock.Text = stockActual.ToString();
                    }
                    catch (Exception exStock)
                    {
                        Log.Escribir($"Error al obtener stock para producto ID {idProducto} vía SP en CargarDatosProducto: {exStock.Message}", exStock);
                        txtStock.Text = "Error"; // Indicar un error en la UI
                        lblMensaje.Text = "Error al obtener el stock del producto.";
                        lblMensaje.CssClass = "mensaje error";
                    }
                }
                else
                {
                    lblMensaje.Text = $"Producto con ID {idProducto} no encontrado. Puede agregar uno nuevo.";
                    lblMensaje.CssClass = "mensaje info";
                    ConfigurarModoAgregar();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar datos del producto con ID {idProducto}: {ex.Message}", ex);
                lblMensaje.Text = "Error al cargar el producto. Verifique los logs para más detalles.";
                lblMensaje.CssClass = "mensaje error";
                ConfigurarModoAgregar();
            }
        }

        // --- Button Click Event Handlers ---

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Manual validation because the ASP.NET validators were removed from the ASPX
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                lblMensaje.Text = "Por favor, complete al menos el Nombre, Precio y Stock del producto.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            // Attempt to parse numerical values
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                lblMensaje.Text = "El precio debe ser un número válido.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }
            if (!int.TryParse(txtStock.Text, out int stock))
            {
                lblMensaje.Text = "El stock debe ser un número entero válido.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            try
            {
                // Check if a product with the same name already exists
                var existingProduct = _productoRepository.GetByNombre(txtNombre.Text.Trim());
                if (existingProduct != null)
                {
                    lblMensaje.Text = $"Ya existe un producto con el nombre '{txtNombre.Text.Trim()}'. Por favor, use un nombre diferente.";
                    lblMensaje.CssClass = "mensaje error";
                    return;
                }

                // Llamar al método del repositorio que usa el SP IN
                _productoRepository.InsertarProductoViaSP(
                    txtNombre.Text.Trim(),
                    txtDescripcion.Text.Trim(),
                    precio,
                    stock
                );

                lblMensaje.Text = "Producto agregado correctamente utilizando procedimiento almacenado.";
                lblMensaje.CssClass = "mensaje exito";
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al agregar producto: " + ex.Message, ex);
                lblMensaje.Text = "Error al agregar producto. Consulte los logs para más detalles.";
                lblMensaje.CssClass = "mensaje error";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdProducto.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                lblMensaje.Text = "Para actualizar, el ID, Nombre, Precio y Stock son obligatorios.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            // Attempt to parse numerical values
            if (!int.TryParse(txtIdProducto.Text, out int idProducto))
            {
                lblMensaje.Text = "ID de producto inválido para actualizar.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                lblMensaje.Text = "El precio debe ser un número válido.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }
            if (!int.TryParse(txtStock.Text, out int nuevoStockDeseado)) // Renombrado a nuevoStockDeseado para claridad
            {
                lblMensaje.Text = "El stock debe ser un número entero válido.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            try
            {
                // Check for duplicate name, excluding the current product being updated
                var existingProduct = _productoRepository.GetByNombre(txtNombre.Text.Trim());
                if (existingProduct != null && existingProduct.id_producto != idProducto)
                {
                    lblMensaje.Text = $"Ya existe otro producto con el nombre '{txtNombre.Text.Trim()}'. Por favor, use un nombre diferente.";
                    lblMensaje.CssClass = "mensaje error";
                    return;
                }

                // --- Lógica para el procedimiento INOUT ---
                // 1. Obtener el stock actual del producto ANTES de la actualización
                int stockActualExistente = _productoRepository.ObtenerStockProductoViaSP(idProducto);

                // 2. Calcular la cantidad de ajuste
                //    Si el nuevo stock deseado es 100 y el actual es 80, el ajuste es +20.
                //    Si el nuevo stock deseado es 50 y el actual es 80, el ajuste es -30.
                int cantidadAjuste = nuevoStockDeseado - stockActualExistente;

                // 3. Actualizar el stock utilizando el procedimiento almacenado INOUT
                int stockFinalSP = _productoRepository.ActualizarStockProductoViaSP(idProducto, cantidadAjuste);

                // 4. Actualizar los otros campos del producto (nombre, descripción, precio)
                //    Tu procedimiento ActualizarStockProducto solo actualiza el stock.
                //    Si necesitas actualizar otros campos, tu método _productoRepository.Update()
                //    (del GenericRepository) es el lugar adecuado para eso.
                //    Asegúrate de que tu modelo 'producto' tenga el stock actualizado si lo necesitas para la operación de Update.
                var productoModificado = new producto
                {
                    id_producto = idProducto,
                    nombre = txtNombre.Text.Trim(),
                    descripcion = txtDescripcion.Text.Trim(),
                    precio = precio,
                    // El stock no se pasa aquí si el SP INOUT es la única fuente de verdad para stock
                    // Pero si tu _productoRepository.Update() actualiza todos los campos,
                    // podrías pasarlo para mantener la consistencia del objeto.
                    stock = stockFinalSP // Usar el stock devuelto por el SP INOUT
                };
                // Si tu Update() genérico actualiza todos los campos, y quieres que el stock se actualice
                // solo por el SP INOUT, entonces esta llamada a Update() NO debería incluir el stock.
                // O bien, tu Update() debería ser inteligente para no sobrescribir el stock si ya fue manejado por un SP.
                // Por simplicidad, asumiremos que Update() actualiza todos los campos, y le pasamos el stock ya ajustado.
                _productoRepository.Update(productoModificado);


                lblMensaje.Text = $"Producto actualizado correctamente. Nuevo stock: {stockFinalSP}.";
                lblMensaje.CssClass = "mensaje exito";
                // LimpiarCampos(); // No limpiar si el usuario espera ver los datos actualizados
                // ConfigurarModoAgregar(); // No cambiar de modo si el usuario está editando
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al actualizar producto con ID {idProducto}: {ex.Message}", ex);
                lblMensaje.Text = "Error al actualizar producto. Consulte los logs para más detalles.";
                lblMensaje.CssClass = "mensaje error";
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdProducto.Text))
            {
                lblMensaje.Text = "Ingrese el ID del producto a eliminar.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            int idProducto;
            if (!int.TryParse(txtIdProducto.Text, out idProducto))
            {
                lblMensaje.Text = "ID de producto inválido para eliminar.";
                lblMensaje.CssClass = "mensaje error";
                return;
            }

            try
            {
                var productoAEliminar = new producto { id_producto = idProducto };
                _productoRepository.Delete(productoAEliminar);
                lblMensaje.Text = "Producto eliminado correctamente.";
                lblMensaje.CssClass = "mensaje exito";
                LimpiarCampos();
                ConfigurarModoAgregar(); // Switch back to Add mode after deletion
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1451) // MySQL error code for foreign key constraint violation
                {
                    lblMensaje.Text = "No se puede eliminar el producto porque tiene registros asociados (ej. en ventas, compras). Elimine los registros relacionados primero.";
                    lblMensaje.CssClass = "mensaje error";
                }
                else
                {
                    Log.Escribir($"Error de base de datos al eliminar producto con ID {idProducto}: {ex.Message}", ex);
                    lblMensaje.Text = "Error al eliminar producto. Consulte los logs para más detalles.";
                    lblMensaje.CssClass = "mensaje error";
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar producto con ID {idProducto}: {ex.Message}", ex);
                lblMensaje.Text = "Error al eliminar producto. Consulte los logs.";
                lblMensaje.CssClass = "mensaje error";
            }
        }

        // --- Utility Method ---

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarModoAgregar();
        }

        private void LimpiarCampos()
        {
            txtIdProducto.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtPrecio.Text = string.Empty;
            txtStock.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            lblMensaje.CssClass = "mensaje"; // Reset CSS class
        }

        // Handles the logout link click (copied from proveedores.aspx.cs)
        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx"); // Redirect to your login page
        }
    }
}