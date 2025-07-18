using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories; // Asegúrate de que esta ruta sea correcta para tus repositorios
using ferreteria_je.Repositories.Models; // Asegúrate de que esta ruta sea correcta para tus modelos (factura, cliente)
using ferreteria_je.Utilidades; // Para el manejo de logs, asumiendo que tienes esta clase

namespace ferreteria_je
{
    public partial class gestionfacturas : ferreteria_je.session.BasePage // Hereda de BasePage si gestionproveedores.aspx.cs lo hace
    {
        // Instancias de los repositorios
        private FacturaRepository _facturaRepository;
        private ClienteRepository _clienteRepository; // Necesitarás un repositorio de clientes

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inicializa los repositorios en cada carga de página
            _facturaRepository = new FacturaRepository();
            _clienteRepository = new ClienteRepository(); // Asegúrate de que ClienteRepository exista y sea correcto

            if (!IsPostBack)
            {
                // Carga los clientes en el DropDownList al cargar la página por primera vez
                CargarClientesEnDropDownList();

                // Verifica si se pasó un ID de factura en la URL (modo edición)
                if (Request.QueryString["id"] != null)
                {
                    int idFactura;
                    // Intenta parsear el ID de la QueryString
                    if (int.TryParse(Request.QueryString["id"], out idFactura))
                    {
                        CargarFacturaParaEdicion(idFactura); // Carga los datos de la factura para edición
                        ConfigurarModoEdicion(); // Ajusta la visibilidad de los botones
                    }
                    else
                    {
                        // ID inválido en la URL
                        lblMensaje.Text = "ID de factura inválido en la URL.";
                        ConfigurarModoAgregar(); // Si el ID es inválido, vuelve al modo agregar
                    }
                }
                else
                {
                    // No hay ID en la URL, asume modo "Agregar Nueva"
                    ConfigurarModoAgregar();
                }
            }
        }

        /// <summary>
        /// Carga los clientes de la base de datos en el DropDownList.
        /// </summary>
        private void CargarClientesEnDropDownList()
        {
            try
            {
                var clientes = _clienteRepository.GetAll().ToList(); // Asume que GetAll() devuelve una IEnumerable<Cliente>
                ddlCliente.DataSource = clientes;
                ddlCliente.DataTextField = "nombre"; // Asume que tu modelo Cliente tiene una propiedad 'nombre_completo'
                ddlCliente.DataValueField = "id_cliente"; // Asume que tu modelo Cliente tiene una propiedad 'id_cliente'
                ddlCliente.DataBind();

                // Añade un elemento por defecto si no quieres que el primero sea seleccionable directamente
                ddlCliente.Items.Insert(0, new ListItem("Seleccione un cliente", "0"));
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar clientes en DropDownList: " + ex.Message, ex);
                lblMensaje.Text = "Error al cargar los clientes. Consulte los logs.";
            }
        }

        /// <summary>
        /// Carga los datos de una factura específica en los campos del formulario para su edición.
        /// </summary>
        /// <param name="idFactura">El ID de la factura a cargar.</param>
        private void CargarFacturaParaEdicion(int idFactura)
        {
            try
            {
                var facturaEncontrada = _facturaRepository.Get(f => f.id_factura == idFactura); // Asume método Get en tu repositorio

                if (facturaEncontrada != null)
                {
                    txtIdFactura.Text = facturaEncontrada.id_factura.ToString();
                    txtFecha.Text = facturaEncontrada.fecha.ToString("yyyy-MM-dd"); // Formato para input type="date"
                    txtTotal.Text = facturaEncontrada.total.ToString();

                    // Seleccionar el cliente correcto en el DropDownList
                    ListItem clienteItem = ddlCliente.Items.FindByValue(facturaEncontrada.id_cliente.ToString());
                    if (clienteItem != null)
                    {
                        ddlCliente.ClearSelection();
                        clienteItem.Selected = true;
                    }

                    lblMensaje.Text = "Factura encontrada. Puede modificarla y hacer clic en Actualizar.";
                }
                else
                {
                    lblMensaje.Text = $"Factura con ID {idFactura} no encontrada. Creando una nueva.";
                    ConfigurarModoAgregar(); // Si no se encuentra, vuelve al modo agregar
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar datos de la factura con ID {idFactura}: {ex.Message}", ex);
                lblMensaje.Text = "Error al cargar la factura. Verifique los logs para más detalles.";
                ConfigurarModoAgregar(); // En caso de error, también vuelve al modo agregar
            }
        }

        /// <summary>
        /// Configura la visibilidad de los botones y el estado de los campos para el modo "Agregar Nueva Factura".
        /// </summary>
        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            txtIdFactura.ReadOnly = true; // El ID de factura es autoincremental
            LimpiarCampos(); // Limpia los campos para una nueva factura
            lblMensaje.Text = "Ingrese los datos para una nueva factura.";
        }

        /// <summary>
        /// Configura la visibilidad de los botones para el modo "Editar/Eliminar Factura".
        /// </summary>
        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            txtIdFactura.ReadOnly = true; // El ID de factura no debe ser editable
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtFecha.Text) || string.IsNullOrWhiteSpace(txtTotal.Text) || ddlCliente.SelectedValue == "0")
            {
                lblMensaje.Text = "Por favor, complete la Fecha, el Total y seleccione un Cliente para la factura.";
                return;
            }

            try
            {
                // Crear un nuevo objeto Factura con los datos del formulario
                var nuevaFactura = new factura // Asume que tienes un modelo Factura
                {
                    fecha = DateTime.Parse(txtFecha.Text), // Convertir texto a DateTime
                    id_cliente = int.Parse(ddlCliente.SelectedValue), // Obtener el ID del cliente seleccionado
                    total = decimal.Parse(txtTotal.Text) // Convertir texto a decimal
                };

                _facturaRepository.Add(nuevaFactura); // Usar el método Add del repositorio
                lblMensaje.Text = "Factura agregada correctamente.";
                LimpiarCampos(); // Limpiar campos después de agregar
                ConfigurarModoAgregar(); // Volver al modo agregar
            }
            catch (FormatException)
            {
                lblMensaje.Text = "Formato de fecha o total incorrecto. Verifique los valores.";
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al agregar factura: " + ex.Message, ex);
                lblMensaje.Text = "Error al agregar factura. Consulte los logs.";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            // Para actualizar, necesitamos un ID de factura válido y que los campos obligatorios no estén vacíos
            if (string.IsNullOrWhiteSpace(txtIdFactura.Text) ||
                string.IsNullOrWhiteSpace(txtFecha.Text) || string.IsNullOrWhiteSpace(txtTotal.Text) || ddlCliente.SelectedValue == "0")
            {
                lblMensaje.Text = "Para actualizar, ingrese un ID, la Fecha, el Total y seleccione un Cliente.";
                return;
            }

            int idFactura;
            if (!int.TryParse(txtIdFactura.Text, out idFactura))
            {
                lblMensaje.Text = "ID de factura inválido.";
                return;
            }

            try
            {
                // Crear un objeto Factura con los datos modificados
                var facturaModificada = new factura
                {
                    id_factura = idFactura, // CRUCIAL: Asignar el ID para que Dapper.Contrib sepa qué actualizar
                    fecha = DateTime.Parse(txtFecha.Text),
                    id_cliente = int.Parse(ddlCliente.SelectedValue),
                    total = decimal.Parse(txtTotal.Text)
                };

                _facturaRepository.Update(facturaModificada); // Usar el método Update del repositorio
                lblMensaje.Text = "Factura actualizada correctamente.";
                // Opcional: Recargar los datos de la factura si hay reglas de negocio o actualizaciones automáticas en DB
                // CargarFacturaParaEdicion(idFactura);
            }
            catch (FormatException)
            {
                lblMensaje.Text = "Formato de fecha o total incorrecto. Verifique los valores.";
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al actualizar factura con ID {idFactura}: {ex.Message}", ex);
                lblMensaje.Text = "Error al actualizar factura. Consulte los logs.";
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdFactura.Text))
            {
                lblMensaje.Text = "Ingrese el ID de la factura a eliminar.";
                return;
            }

            int idFactura;
            if (!int.TryParse(txtIdFactura.Text, out idFactura))
            {
                lblMensaje.Text = "ID de factura inválido para eliminar.";
                return;
            }

            try
            {
                // Para eliminar con Dapper.Contrib, necesitamos un objeto del tipo T
                // con la propiedad marcada con [Key] (id_factura) populada.
                var facturaAEliminar = new factura { id_factura = idFactura };

                _facturaRepository.Delete(facturaAEliminar); // Usar el método Delete del repositorio
                lblMensaje.Text = "Factura eliminada correctamente.";
                LimpiarCampos(); // Limpiar campos después de la eliminación
                ConfigurarModoAgregar(); // Volver al modo agregar
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar factura con ID {idFactura}: {ex.Message}", ex);
                lblMensaje.Text = "Error al eliminar factura. Consulte los logs.";
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarModoAgregar(); // Al limpiar, volvemos al modo de agregar nueva factura
        }

        /// <summary>
        /// Limpia todos los campos de texto del formulario y la etiqueta de mensaje.
        /// </summary>
        private void LimpiarCampos()
        {
            txtIdFactura.Text = string.Empty;
            txtFecha.Text = string.Empty; // Se rellenará automáticamente por JS en DOMContentLoaded
            txtTotal.Text = string.Empty;

            // Asegúrate de limpiar la selección actual antes de intentar seleccionar un nuevo ítem
            ddlCliente.ClearSelection();

            // Intenta encontrar el ListItem con valor "0"
            ListItem defaultItem = ddlCliente.Items.FindByValue("0");

            // Verifica si el ítem existe antes de intentar seleccionarlo
            if (defaultItem != null)
            {
                defaultItem.Selected = true; // Selecciona el "Seleccione un cliente"
            }
            else
            {
                // Si por alguna razón el ítem "0" no se cargó, puedes optar por:
                // 1. Recargar los clientes (siempre y cuando sea seguro y no cause loops)
                // 2. Simplemente dejarlo sin seleccionar o seleccionar el primero disponible si existe
                // 3. (Recomendado) Añadir un log para investigar por qué no se cargó el item "0"
                Log.Escribir("Advertencia: El ListItem con valor '0' (Seleccione un cliente) no se encontró en ddlCliente.Items durante LimpiarCampos().");
                // Opcionalmente, puedes intentar añadirlo si no existe, aunque esto es menos común si ya lo haces en Page_Load
                // ddlCliente.Items.Insert(0, new ListItem("Seleccione un cliente", "0"));
                // ddlCliente.Items.FindByValue("0").Selected = true; // Ahora debería funcionar
            }

            lblMensaje.Text = string.Empty;
        }
    }
}