// ferreteria_je/gestionclientes.aspx.cs
using System;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories; // Importa tu ClienteRepository
using ferreteria_je.Repositories.Models; // Importa tu modelo 'cliente'
using ferreteria_je.Utilidades; // Importa tu clase Log (para manejo de errores)

namespace ferreteria_je
{
    public partial class gestionclientes : ferreteria_je.session.BasePage
    {
        // Instancia del repositorio de Clientes
        private ClienteRepository _clienteRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inicializa el repositorio en cada carga de página
            _clienteRepository = new ClienteRepository();

            if (!IsPostBack)
            {
                // Limpiar campos al cargar la página por primera vez
                LimpiarCampos();

                // Verificar si se pasó un ID de cliente en la URL (modo edición desde clientes.aspx)
                if (Request.QueryString["id"] != null)
                {
                    int idCliente;
                    if (int.TryParse(Request.QueryString["id"], out idCliente))
                    {
                        CargarDatosCliente(idCliente); // Cargar datos del cliente para edición
                        ConfigurarModoEdicion();     // Configurar la interfaz para edición
                    }
                    else
                    {
                        // ID inválido en la URL, revertir a modo agregar
                        lblMessage.Text = "ID de cliente inválido en la URL.";
                        lblMessage.CssClass = "message-label error";
                        ConfigurarModoAgregar();
                    }
                }
                else
                {
                    // No hay ID en la URL, se asume modo "Agregar Nuevo Cliente"
                    ConfigurarModoAgregar();
                }
            }
        }

        /// <summary>
        /// Configura los botones y campos para el modo "Agregar Nuevo Cliente".
        /// </summary>
        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            btnConsultar.Visible = true; // El botón de consultar siempre es útil para buscar antes de agregar.

            // El campo txtCedulaBusqueda es para buscar un cliente existente.
            // En modo "Agregar", es útil para verificar si un cliente ya existe antes de crearlo.
            txtCedulaBusqueda.Visible = true;

            // Si txtCedula se usa para el nuevo cliente, no debería ser ReadOnly.
            txtCedula.ReadOnly = false;
            lblMessage.Text = "Ingrese los datos para un nuevo cliente. Puede usar 'Consultar' para buscar uno existente por cédula.";
            lblMessage.CssClass = "message-label info";
            LimpiarCampos(); // Limpiar campos para un nuevo registro
        }

        /// <summary>
        /// Configura los botones y campos para el modo "Editar/Eliminar Cliente".
        /// </summary>
        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            btnConsultar.Visible = true; // Mantener visible para permitir buscar otros clientes
            txtCedulaBusqueda.Visible = true; // Es la clave de búsqueda principal en esta página

            // Cuando se edita, la cédula puede ser editable si se permite cambiar la clave principal.
            // Si la cédula es la clave principal y no debe cambiarse directamente, hazla ReadOnly.
            // Para este ejemplo, la mantendremos editable si la BD permite el cambio de la PK.
            txtCedula.ReadOnly = false;

            lblMessage.Text = "Cliente encontrado. Puede modificar los datos o eliminarlo.";
            lblMessage.CssClass = "message-label info";
        }

        /// <summary>
        /// Carga los datos de un cliente específico en los campos del formulario.
        /// Se usa tanto al llegar con un ID en la URL como al buscar por cédula.
        /// </summary>
        /// <param name="idCliente">El ID del cliente a cargar.</param>
        private void CargarDatosCliente(int idCliente)
        {
            try
            {
                cliente clienteEncontrado = _clienteRepository.Get(c => c.id_cliente == idCliente);

                if (clienteEncontrado != null)
                {
                    // Rellenar los campos del formulario con los datos del cliente
                    txtCedulaBusqueda.Text = clienteEncontrado.cedula; // Para que la cédula de búsqueda refleje el cliente cargado
                    txtNombre.Text = clienteEncontrado.nombre;
                    txtCedula.Text = clienteEncontrado.cedula; // La cédula del cliente
                    txtDireccion.Text = clienteEncontrado.direccion;
                    txtEmail.Text = clienteEncontrado.email;
                    txtTelefono.Text = clienteEncontrado.telefono;
                }
                else
                {
                    lblMessage.Text = $"Cliente con ID {idCliente} no encontrado.";
                    lblMessage.CssClass = "message-label warning";
                    LimpiarCampos();
                    ConfigurarModoAgregar(); // Si no se encuentra, volver al modo agregar
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar datos del cliente con ID {idCliente}: {ex.Message}", ex);
                lblMessage.Text = "Error al cargar el cliente. Verifique los logs para más detalles.";
                lblMessage.CssClass = "message-label error";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón "Agregar".
        /// Valida y agrega un nuevo cliente.
        /// </summary>
        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            lblMessage.Text = ""; // Limpiar mensajes anteriores

            // Validar campos obligatorios: nombre y cédula
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                lblMessage.Text = "Por favor, complete los campos obligatorios: Nombre y Cédula.";
                lblMessage.CssClass = "message-label warning";
                return;
            }

            try
            {
                // Crear el objeto cliente con los datos del formulario
                cliente nuevoCliente = new cliente
                {
                    nombre = txtNombre.Text.Trim(),
                    cedula = txtCedula.Text.Trim(), // Cédula del nuevo cliente
                    direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim()
                };

                // Verificar si ya existe un cliente con esa cédula antes de agregar
                cliente clienteExistente = _clienteRepository.GetByCedula(nuevoCliente.cedula);
                if (clienteExistente != null)
                {
                    lblMessage.Text = "Ya existe un cliente con la cédula ingresada. Considere actualizar el cliente existente.";
                    lblMessage.CssClass = "message-label warning";
                    return;
                }

                _clienteRepository.Add(nuevoCliente); // Usar el método Add del repositorio genérico
                lblMessage.Text = "Cliente agregado exitosamente.";
                lblMessage.CssClass = "message-label success";
                LimpiarCampos(); // Limpiar el formulario después de agregar
                ConfigurarModoAgregar(); // Volver al modo de agregar
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al agregar cliente: " + ex.Message, ex);
                lblMessage.Text = "Error al agregar cliente. Verifique los logs para más detalles.";
                lblMessage.CssClass = "message-label error";
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón "Actualizar".
        /// Valida y actualiza los datos de un cliente existente.
        /// </summary>
        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Para actualizar, necesitamos la cédula del campo txtCedula (que se supone contiene el ID o la cédula original)
            // o de txtCedulaBusqueda si el usuario la modificó allí y consultó.
            // Priorizamos la cédula del formulario principal para la actualización de datos.
            string cedulaActualizar = txtCedula.Text.Trim(); // La cédula del campo de datos del formulario

            if (string.IsNullOrWhiteSpace(cedulaActualizar))
            {
                lblMessage.Text = "Para actualizar, el campo 'Cédula' no puede estar vacío.";
                lblMessage.CssClass = "message-label warning";
                return;
            }

            // Validar campos obligatorios del formulario
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblMessage.Text = "El nombre del cliente es obligatorio.";
                lblMessage.CssClass = "message-label warning";
                return;
            }

            try
            {
                // Primero, obtener el cliente original por la cédula que se usa para buscar/identificar
                // Esto es crucial para obtener el id_cliente que Dapper.Contrib necesita.
                cliente clienteOriginal = _clienteRepository.GetByCedula(txtCedulaBusqueda.Text.Trim());

                if (clienteOriginal == null)
                {
                    lblMessage.Text = "No se encontró un cliente para actualizar con la cédula de búsqueda. Por favor, consulte primero.";
                    lblMessage.CssClass = "message-label info";
                    return;
                }

                // Crear el objeto cliente con los nuevos datos, usando el id_cliente del cliente original
                cliente clienteModificado = new cliente
                {
                    id_cliente = clienteOriginal.id_cliente, // CRUCIAL: Asignar el ID para que Dapper.Contrib sepa qué fila actualizar
                    nombre = txtNombre.Text.Trim(),
                    cedula = txtCedula.Text.Trim(), // La cédula podría haber sido modificada en el formulario
                    direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim()
                };

                // Verificar si la nueva cédula (si fue cambiada) ya existe en OTRO cliente
                cliente clienteConMismaCedula = _clienteRepository.GetByCedula(clienteModificado.cedula);
                if (clienteConMismaCedula != null && clienteConMismaCedula.id_cliente != clienteModificado.id_cliente)
                {
                    lblMessage.Text = "La nueva cédula ya está asignada a otro cliente. Por favor, elija una cédula única.";
                    lblMessage.CssClass = "message-label warning";
                    return;
                }

                _clienteRepository.Update(clienteModificado); // Usar el método Update del repositorio genérico
                lblMessage.Text = "Cliente actualizado exitosamente.";
                lblMessage.CssClass = "message-label success";
                // Después de actualizar, recargar los datos para asegurar que se muestren los últimos cambios
                // y para que el txtCedulaBusqueda se actualice si la cédula fue cambiada.
                CargarDatosCliente(clienteOriginal.id_cliente);
                ConfigurarModoEdicion(); // Mantener en modo edición
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al actualizar cliente: {ex.Message}", ex);
                lblMessage.Text = "Error al actualizar cliente. Verifique los logs para más detalles.";
                lblMessage.CssClass = "message-label error";
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón "Eliminar".
        /// Elimina un cliente de la base de datos utilizando la cédula de búsqueda.
        /// </summary>
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            string cedulaAEliminar = txtCedulaBusqueda.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedulaAEliminar))
            {
                lblMessage.Text = "Para eliminar, ingrese la Cédula del cliente en el campo de búsqueda.";
                lblMessage.CssClass = "message-label warning";
                return;
            }

            try
            {
                // Primero, encontrar el cliente por cédula para obtener su ID
                cliente clienteExistente = _clienteRepository.GetByCedula(cedulaAEliminar);

                if (clienteExistente != null)
                {
                    // Crear un objeto cliente con el ID para la eliminación
                    var clienteParaEliminar = new cliente { id_cliente = clienteExistente.id_cliente };

                    _clienteRepository.Delete(clienteParaEliminar); // Usar el método Delete del repositorio genérico
                    lblMessage.Text = "Cliente eliminado exitosamente.";
                    lblMessage.CssClass = "message-label success";
                    LimpiarCampos(); // Limpiar el formulario después de eliminar
                    ConfigurarModoAgregar(); // Volver al modo de agregar
                }
                else
                {
                    lblMessage.Text = "No se encontró un cliente con la Cédula especificada para eliminar.";
                    lblMessage.CssClass = "message-label info";
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar cliente con cédula {cedulaAEliminar}: {ex.Message}", ex);

                // MySqlException con número 1451 indica una restricción de clave foránea
                if (ex.InnerException != null && ex.InnerException.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                {
                    lblMessage.Text = "Error: No se puede eliminar el cliente porque tiene ventas u otros registros asociados. Elimine los registros asociados primero.";
                    lblMessage.CssClass = "message-label error";
                }
                else
                {
                    lblMessage.Text = "Error al eliminar cliente. Verifique los logs para más detalles.";
                    lblMessage.CssClass = "message-label error";
                }
            }
        }

        /// <summary>
        /// Maneja el evento de clic del botón "Consultar".
        /// Consulta un cliente por su cédula y carga sus datos en el formulario.
        /// </summary>
        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            string cedulaBusqueda = txtCedulaBusqueda.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedulaBusqueda))
            {
                lblMessage.Text = "Por favor, ingrese la Cédula del cliente a consultar.";
                lblMessage.CssClass = "message-label warning";
                return;
            }

            try
            {
                cliente clienteEncontrado = _clienteRepository.GetByCedula(cedulaBusqueda);

                if (clienteEncontrado != null)
                {
                    CargarDatosCliente(clienteEncontrado.id_cliente); // Cargar los datos encontrados en los TextBoxes
                    ConfigurarModoEdicion(); // Configurar botones para edición/eliminación
                }
                else
                {
                    lblMessage.Text = "No se encontró ningún cliente con la Cédula especificada.";
                    lblMessage.CssClass = "message-label info";
                    LimpiarCampos(); // Limpiar campos si no se encuentra
                    ConfigurarModoAgregar(); // Volver al modo de agregar si la búsqueda no arroja resultados
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al consultar cliente por cédula '{cedulaBusqueda}': {ex.Message}", ex);
                lblMessage.Text = "Error al consultar cliente. Verifique los logs para más detalles.";
                lblMessage.CssClass = "message-label error";
            }
        }

        /// <summary>
        /// Limpia todos los campos del formulario y el mensaje.
        /// </summary>
        private void LimpiarCampos()
        {
            txtCedulaBusqueda.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtCedula.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            lblMessage.Text = string.Empty; // Limpiar también el mensaje
        }

        // Si tienes un botón "Limpiar" o "Nuevo" en tu ASPX, así se manejaría:
        // protected void btnLimpiar_Click(object sender, EventArgs e)
        // {
        //     LimpiarCampos();
        //     ConfigurarModoAgregar(); // Cuando se limpia, se vuelve al modo de agregar
        // }

        // Si tienes un botón para regresar a clientes.aspx
        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("clientes.aspx");
        }
    }
}