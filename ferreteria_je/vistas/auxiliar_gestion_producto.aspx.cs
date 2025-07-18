using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace ferreteria_je
{
    public partial class auxiliar_gestion_producto : ferreteria_je.session.BasePage
    {
        private IProductoRepository _productoRepository;

        // Declaraciones de controles de UI (already present and correct)
        // protected System.Web.UI.WebControls.TextBox txtNombreProducto;
        // protected System.Web.UI.WebControls.TextBox txtDescripcion;
        // protected System.Web.UI.WebControls.TextBox txtPrecio;
        // protected System.Web.UI.WebControls.TextBox txtStock;
        // protected System.Web.UI.WebControls.LinkButton btnAgregar;
        // protected System.Web.UI.WebControls.LinkButton btnLimpiar;
        // protected System.Web.UI.WebControls.LinkButton btnVolver;
        // protected System.Web.UI.WebControls.LinkButton btnActualizar;
        // protected System.Web.UI.WebControls.Label lblMensaje;

        // NUEVO: Propiedad para almacenar el ID del producto actual (para edición)
        public int? CurrentProductoId
        {
            get
            {
                if (ViewState["CurrentProductoId"] != null)
                    return (int)ViewState["CurrentProductoId"];
                return null;
            }
            set
            {
                ViewState["CurrentProductoId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _productoRepository = new ProductoRepository();

            if (!IsPostBack)
            {
                // Verificar si se pasó un ID de producto para edición
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int idToLoad))
                    {
                        CurrentProductoId = idToLoad;
                        CargarDatosProducto(idToLoad);
                        // Modo "Actualizar": Mostrar botón Actualizar, ocultar Agregar
                        btnActualizar.Visible = true;
                        btnAgregar.Visible = false;
                    }
                    else
                    {
                        // ID inválido en la URL, se comporta como si fuera para agregar
                        MostrarMensaje("ID de producto inválido en la URL. Ingresando en modo 'Agregar'.", "warning");
                        LimpiarCampos();
                        btnActualizar.Visible = true;
                        btnAgregar.Visible = true;
                    }
                }
                else
                {
                    // No hay ID en la URL, se asume modo "Agregar"
                    LimpiarCampos();
                    btnActualizar.Visible = true; // Ocultar el botón Actualizar al inicio
                    btnAgregar.Visible = true; // Mostrar el botón Agregar
                }
            }
        }

        // NUEVO: Método para cargar los datos de un producto por su ID
        private void CargarDatosProducto(int idProducto)
        {
            try
            {
                var productoExistente = _productoRepository.Get(p => p.id_producto == idProducto);
                if (productoExistente != null)
                {
                    txtNombreProducto.Text = productoExistente.nombre;
                    txtDescripcion.Text = productoExistente.descripcion;
                    txtPrecio.Text = productoExistente.precio.ToString();
                    txtStock.Text = productoExistente.stock.ToString();
                    MostrarMensaje($"Producto ID {idProducto} cargado exitosamente para edición.", "info");
                }
                else
                {
                    MostrarMensaje($"No se encontró ningún producto con el ID: {idProducto}. Volviendo a modo 'Agregar'.", "warning");
                    LimpiarCampos();
                    CurrentProductoId = null; // Resetear el ID si no se encontró el producto
                    // Revertir a modo "Agregar"
                    btnActualizar.Visible = true;
                    btnAgregar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar el producto: {ex.Message}", "error");
                LimpiarCampos();
                CurrentProductoId = null;
                btnActualizar.Visible = true;
                btnAgregar.Visible = true;
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Resetear el ID actual para asegurar que se agrega uno nuevo
            CurrentProductoId = null; // Important: Ensure it's null when adding

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                producto nuevoProducto = new producto
                {
                    nombre = txtNombreProducto.Text.Trim(),
                    descripcion = txtDescripcion.Text.Trim(),
                    precio = decimal.Parse(txtPrecio.Text.Trim()),
                    stock = int.Parse(txtStock.Text.Trim())
                };

                // Validaciones adicionales para nuevos productos
                if (nuevoProducto.precio <= 0)
                {
                    MostrarMensaje("El precio del producto debe ser mayor a cero.", "warning");
                    return;
                }
                if (nuevoProducto.stock < 0)
                {
                    MostrarMensaje("El stock del producto no puede ser negativo.", "warning");
                    return;
                }

                _productoRepository.Add(nuevoProducto);

                MostrarMensaje("Producto agregado exitosamente!", "success");
                LimpiarCampos();
            }
            catch (FormatException)
            {
                MostrarMensaje("Error de formato: Por favor, introduce valores numéricos válidos para Precio y Stock.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al agregar el producto: " + ex.Message, "error");
            }
        }

        // NUEVO: Método para el botón Actualizar
        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (CurrentProductoId == null)
            {
                MostrarMensaje("No hay un producto seleccionado para actualizar. Por favor, cargue un producto o agregue uno nuevo.", "warning");
                return;
            }

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                // Obtener el producto existente para actualizar
                var productoToUpdate = _productoRepository.Get(p => p.id_producto == CurrentProductoId.Value);

                if (productoToUpdate == null)
                {
                    MostrarMensaje("El producto que intenta actualizar ya no existe. Limpiando formulario.", "error");
                    LimpiarCampos();
                    CurrentProductoId = null;
                    // Volver al modo de agregar un nuevo producto
                    btnAgregar.Visible = true;
                    btnActualizar.Visible = true;
                    return;
                }

                // Actualizar las propiedades del producto
                productoToUpdate.nombre = txtNombreProducto.Text.Trim();
                productoToUpdate.descripcion = txtDescripcion.Text.Trim();

                if (decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
                {
                    productoToUpdate.precio = precio;
                }
                else
                {
                    MostrarMensaje("El formato del precio no es válido.", "warning");
                    return;
                }

                if (int.TryParse(txtStock.Text.Trim(), out int stock))
                {
                    productoToUpdate.stock = stock;
                }
                else
                {
                    MostrarMensaje("El formato del stock no es válido.", "warning");
                    return;
                }

                // Validaciones adicionales de valores (repetidas para seguridad, aunque ValidarCampos puede cubrirlas en parte)
                if (productoToUpdate.precio <= 0)
                {
                    MostrarMensaje("El precio del producto debe ser mayor a cero.", "warning");
                    return;
                }
                if (productoToUpdate.stock < 0)
                {
                    MostrarMensaje("El stock del producto no puede ser negativo.", "warning");
                    return;
                }

                // Guardar los cambios en el repositorio
                _productoRepository.Update(productoToUpdate);

                MostrarMensaje("¡Producto actualizado exitosamente!", "success");
                LimpiarCampos();
                CurrentProductoId = null; // Después de actualizar, limpiar el contexto de edición
                // Volver al modo de agregar un nuevo producto
                btnAgregar.Visible = true;
                btnActualizar.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al actualizar el producto: " + ex.Message, "error");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MostrarMensaje("Campos limpiados.", "info");
            CurrentProductoId = null; // También limpiar el ID de producto actual
            // Al limpiar, siempre volvemos al modo de agregar nuevo producto
            btnAgregar.Visible = true;
            btnActualizar.Visible = true;
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("auxiliar_producto.aspx"); // Redirige a la página principal de productos
        }

        private void LimpiarCampos()
        {
            txtNombreProducto.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtPrecio.Text = string.Empty;
            txtStock.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            lblMensaje.CssClass = "";
        }

        private bool ValidarCampos()
        {
            // Initial check for empty/whitespace fields
            if (string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                MostrarMensaje("El nombre del producto es obligatorio.", "warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MostrarMensaje("El precio del producto es obligatorio.", "warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MostrarMensaje("El stock del producto es obligatorio.", "warning");
                return false;
            }

            // Numeric format validation for Price and Stock
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
            {
                MostrarMensaje("El precio debe ser un número válido.", "warning");
                return false;
            }
            if (!int.TryParse(txtStock.Text.Trim(), out int stock))
            {
                MostrarMensaje("El stock debe ser un número entero válido.", "warning");
                return false;
            }

            // Value range validation
            if (precio <= 0)
            {
                MostrarMensaje("El precio del producto debe ser mayor a cero.", "warning");
                return false;
            }
            if (stock < 0)
            {
                MostrarMensaje("El stock del producto no puede ser negativo.", "warning");
                return false;
            }

            return true;
        }

        // Método auxiliar para mostrar mensajes con estilos (already present and good)
        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = "mensaje"; // Clase base
            switch (tipo)
            {
                case "success":
                    lblMensaje.CssClass += " success";
                    break;
                case "error":
                    lblMensaje.CssClass += " error";
                    break;
                case "warning":
                    lblMensaje.CssClass += " alert-warning"; // Using 'alert-warning' for consistency
                    break;
                case "info":
                    lblMensaje.CssClass += " alert-info";     // Using 'alert-info' for consistency
                    break;
                default:
                    lblMensaje.CssClass += " alert-info";
                    break;
            }
        }
    }
}