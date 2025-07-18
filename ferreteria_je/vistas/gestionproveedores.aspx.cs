using System;
using System.Configuration; // Still used for ConfigurationManager if needed elsewhere, though connection string handled by repo.
using ferreteria_je.Repositories; // Import your ProveedorRepository
using ferreteria_je.Repositories.Models; // Import your 'proveedor' model
using ferreteria_je.Utilidades; // Import your Log class (for error handling)
using System.Web.UI; // For Page
using System.Web.UI.WebControls; // For TextBox, Label, Button

namespace ferreteria_je
{
    public partial class gestionproveedores : ferreteria_je.session.BasePage
    {
        // Instance of the Supplier Repository.
        // Initialized in Page_Load to ensure it's available after the initial page load.
        private ProveedorRepository _proveedorRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Always initialize the repository in Page_Load or in the constructor if it were a service.
            _proveedorRepository = new ProveedorRepository();

            if (!IsPostBack)
            {
                // Check if an ID was passed in the URL (indicates "Edit" or "View Detail" mode)
                if (Request.QueryString["id"] != null)
                {
                    int idProveedor;
                    // Attempt to parse the ID from the QueryString
                    if (int.TryParse(Request.QueryString["id"], out idProveedor))
                    {
                        CargarDatosProveedor(idProveedor); // Load supplier data for editing
                        ConfigurarModoEdicion(); // Adjust button visibility
                    }
                    else
                    {
                        // Invalid ID in the URL
                        lblMensaje.Text = "ID de proveedor inválido en la URL.";
                        ConfigurarModoAgregar(); // If ID is invalid, revert to add mode
                    }
                }
                else
                {
                    // No ID in the URL, assume "Add New" mode
                    ConfigurarModoAgregar();
                }
            }
        }

        /// <summary>
        /// Configures buttons and fields for "Add New Supplier" mode.
        /// </summary>
        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            // txtIdProveedor is auto-incremented in the DB, so we make it read-only or hide it.
            // To maintain a consistent interface, we leave it visible but non-editable.
            txtIdProveedor.ReadOnly = true;
            LimpiarCampos(); // Clear fields for a new record
            lblMensaje.Text = "Ingrese los datos para un nuevo proveedor.";
        }

        /// <summary>
        /// Configures buttons and fields for "Edit/Delete Supplier" mode.
        /// </summary>
        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            txtIdProveedor.ReadOnly = true; // The ID should not be editable in edit mode
        }

        /// <summary>
        /// Loads data for a specific supplier into the form fields.
        /// </summary>
        /// <param name="idProveedor">The ID of the supplier to load.</param>
        private void CargarDatosProveedor(int idProveedor)
        {
            try
            {
                // We use the Get(predicate) method of GenericRepository to search by ID.
                // It's efficient as it filters a list already loaded by GetAll() if used.
                // A more direct alternative would be if GenericRepository had GetById(int id).
                var proveedorEncontrado = _proveedorRepository.Get(p => p.id_proveedor == idProveedor);

                if (proveedorEncontrado != null)
                {
                    txtIdProveedor.Text = proveedorEncontrado.id_proveedor.ToString();
                    txtNombre.Text = proveedorEncontrado.nombre;
                    txtDireccion.Text = proveedorEncontrado.direccion;
                    txtTelefono.Text = proveedorEncontrado.telefono;
                    txtEmail.Text = proveedorEncontrado.email;
                    lblMensaje.Text = "Proveedor encontrado. Puede modificarlo y hacer clic en Actualizar.";
                }
                else
                {
                    lblMensaje.Text = $"Proveedor con ID {idProveedor} no encontrado. Creando uno nuevo.";
                    ConfigurarModoAgregar(); // If not found, revert to add mode
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar datos del proveedor con ID {idProveedor}: {ex.Message}", ex);
                lblMensaje.Text = "Error al cargar el proveedor. Verifique los logs para más detalles.";
                ConfigurarModoAgregar(); // In case of error, also revert to add mode
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validate that required fields for adding are not empty.
            // Assuming Name and Email are mandatory by the model.
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMensaje.Text = "Por favor, complete al menos el Nombre y el Email del proveedor.";
                return;
            }

            try
            {
                var nuevoProveedor = new proveedor
                {
                    nombre = txtNombre.Text.Trim(),
                    direccion = txtDireccion.Text.Trim(),
                    telefono = txtTelefono.Text.Trim(),
                    email = txtEmail.Text.Trim()
                };

                _proveedorRepository.Add(nuevoProveedor); // Use the Add method of the repository
                lblMensaje.Text = "Proveedor agregado correctamente.";
                LimpiarCampos(); // Clear fields after adding
                // Optional: Redirect to the supplier list
                // Response.Redirect("proveedores.aspx");
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al agregar proveedor: " + ex.Message, ex);
                lblMensaje.Text = "Error al agregar proveedor. Consulte los logs.";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            // To update, we need a valid supplier ID and that mandatory fields are not empty.
            if (string.IsNullOrWhiteSpace(txtIdProveedor.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMensaje.Text = "Para actualizar, ingrese un ID y complete al menos el Nombre y Email.";
                return;
            }

            int idProveedor;
            if (!int.TryParse(txtIdProveedor.Text, out idProveedor))
            {
                lblMensaje.Text = "ID de proveedor inválido.";
                return;
            }

            try
            {
                var proveedorModificado = new proveedor
                {
                    id_proveedor = idProveedor, // CRUCIAL: Assign the ID for Dapper.Contrib to know what to update
                    nombre = txtNombre.Text.Trim(),
                    direccion = txtDireccion.Text.Trim(),
                    telefono = txtTelefono.Text.Trim(),
                    email = txtEmail.Text.Trim()
                };

                _proveedorRepository.Update(proveedorModificado); // Use the Update method of the repository
                lblMensaje.Text = "Proveedor actualizado correctamente.";
                // After updating, you might reload the data (if there are business rules or auto-updates in DB)
                // or redirect to the supplier list.
                // CargarDatosProveedor(idProveedor);
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al actualizar proveedor con ID {idProveedor}: {ex.Message}", ex);
                lblMensaje.Text = "Error al actualizar proveedor. Consulte los logs.";
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdProveedor.Text))
            {
                lblMensaje.Text = "Ingrese el ID del proveedor a eliminar.";
                return;
            }

            int idProveedor;
            if (!int.TryParse(txtIdProveedor.Text, out idProveedor))
            {
                lblMensaje.Text = "ID de proveedor inválido para eliminar.";
                return;
            }

            try
            {
                // To delete with Dapper.Contrib, we need an object of type T
                // with the property marked with [Key] (id_proveedor) populated.
                var proveedorAEliminar = new proveedor { id_proveedor = idProveedor };

                _proveedorRepository.Delete(proveedorAEliminar); // Use the Delete method of the repository
                lblMensaje.Text = "Proveedor eliminado correctamente.";
                LimpiarCampos(); // Clear fields after deletion
                ConfigurarModoAgregar(); // Revert to add mode
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar proveedor con ID {idProveedor}: {ex.Message}", ex);
                lblMensaje.Text = "Error al eliminar proveedor. Consulte los logs.";
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarModoAgregar(); // When clearing, revert to add new mode
        }

        /// <summary>
        /// Clears all text fields on the form and the message label.
        /// </summary>
        private void LimpiarCampos()
        {
            txtIdProveedor.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtEmail.Text = string.Empty;
            lblMensaje.Text = string.Empty;
        }
    }
}