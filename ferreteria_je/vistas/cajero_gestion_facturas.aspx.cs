using ferreteria_je.Repositories.Models;
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces;
using ferreteria_je.Repositories.RepositoriesGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Dapper.Contrib.Extensions; // Make sure this is included for [Write(false)] if needed elsewhere

namespace ferreteria_je
{
    public partial class cajero_gestion_facturas : ferreteria_je.session.BasePage // Assuming BasePage is in session namespace
    {
        // Declare generic repository instances
        private IRepository<factura> _facturaRepository;
        private IRepository<cliente> _clienteRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize repositories
            _facturaRepository = new GenericRepository<factura>();
            _clienteRepository = new GenericRepository<cliente>();

            if (!IsPostBack)
            {
                PopulateDropDownLists(); // Populate dropdowns on initial load

                // Check for Factura ID in query string for edit mode
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int idToLoad))
                    {
                        hdnFacturaId.Value = idToLoad.ToString(); // Store ID in hidden field
                        CargarDatosFactura(idToLoad);
                        btnActualizar.Visible = true;
                        btnAgregar.Visible = false;
                    }
                    else
                    {
                        MostrarMensaje("ID de factura inválido en la URL. Ingresando en modo 'Registrar'.", "warning");
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
                ddlCliente.DataTextField = "nombre"; // Based on your 'cliente' model
                ddlCliente.DataValueField = "id_cliente";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("-- Seleccione Cliente --", "")); // Default option
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar listas desplegables: {ex.Message}", "error");
            }
        }

        private void CargarDatosFactura(int idFactura)
        {
            try
            {
                // Retrieve the existing factura using Get(lambda expression)
                var facturaExistente = _facturaRepository.Get(f => f.id_factura == idFactura);

                if (facturaExistente != null)
                {
                    txtIdFactura.Text = facturaExistente.id_factura.ToString();
                    txtFecha.Text = facturaExistente.fecha.ToString("yyyy-MM-dd"); // Use 'fecha' from your factura model

                    // Handle nullable id_cliente
                    if (facturaExistente.id_cliente.HasValue && ddlCliente.Items.FindByValue(facturaExistente.id_cliente.Value.ToString()) != null)
                        ddlCliente.SelectedValue = facturaExistente.id_cliente.Value.ToString();
                    else if (!facturaExistente.id_cliente.HasValue && ddlCliente.Items.Count > 0)
                        ddlCliente.SelectedIndex = 0; // Select default if null

                    txtTotalFacturado.Text = facturaExistente.total.ToString("F2"); // Use 'total' from your factura model

                    MostrarMensaje($"Factura ID {idFactura} cargada exitosamente para edición.", "info");
                }
                else
                {
                    MostrarMensaje($"No se encontró ninguna factura con el ID: {idFactura}. Volviendo a modo 'Registrar'.", "warning");
                    LimpiarCampos();
                    hdnFacturaId.Value = "";
                    btnActualizar.Visible = true;
                    btnAgregar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la factura: {ex.Message}", "error");
                LimpiarCampos();
                hdnFacturaId.Value = "";
                btnActualizar.Visible = true;
                btnAgregar.Visible = true;
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            hdnFacturaId.Value = ""; // Ensure it's in add mode

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                // Ensure ddlCliente.SelectedValue is not empty for nullable FK
                int? clienteId = null;
                if (!string.IsNullOrEmpty(ddlCliente.SelectedValue))
                {
                    clienteId = int.Parse(ddlCliente.SelectedValue);
                }

                factura nuevaFactura = new factura
                {
                    fecha = DateTime.Parse(txtFecha.Text.Trim()), // Use 'fecha'
                    id_cliente = clienteId, // Assign nullable int? directly
                    total = decimal.Parse(txtTotalFacturado.Text.Trim()) // Use 'total'
                };

                _facturaRepository.Add(nuevaFactura);

                MostrarMensaje("Factura registrada exitosamente!", "success");
                LimpiarCampos();
            }
            catch (FormatException)
            {
                MostrarMensaje("Error de formato: Asegúrese de que todos los campos numéricos y de fecha sean válidos.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar la factura: " + ex.Message, "error");
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hdnFacturaId.Value) || !int.TryParse(hdnFacturaId.Value, out int idFacturaToUpdate))
            {
                MostrarMensaje("No hay una factura seleccionada para actualizar. Por favor, cargue una factura o registre una nueva.", "warning");
                return;
            }

            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                var facturaToUpdate = _facturaRepository.Get(f => f.id_factura == idFacturaToUpdate);

                if (facturaToUpdate == null)
                {
                    MostrarMensaje("La factura que intenta actualizar ya no existe. Limpiando formulario.", "error");
                    LimpiarCampos();
                    hdnFacturaId.Value = "";
                    btnAgregar.Visible = true;
                    btnActualizar.Visible = true;
                    return;
                }

                // Ensure ddlCliente.SelectedValue is not empty for nullable FK
                int? clienteId = null;
                if (!string.IsNullOrEmpty(ddlCliente.SelectedValue))
                {
                    clienteId = int.Parse(ddlCliente.SelectedValue);
                }

                facturaToUpdate.fecha = DateTime.Parse(txtFecha.Text.Trim()); // Use 'fecha'
                facturaToUpdate.id_cliente = clienteId; // Assign nullable int? directly
                facturaToUpdate.total = decimal.Parse(txtTotalFacturado.Text.Trim()); // Use 'total'

                _facturaRepository.Update(facturaToUpdate);

                MostrarMensaje("¡Factura actualizada exitosamente!", "success");
                LimpiarCampos();
                hdnFacturaId.Value = "";
                btnAgregar.Visible = true;
                btnActualizar.Visible = true;
            }
            catch (FormatException)
            {
                MostrarMensaje("Error de formato: Asegúrese de que todos los campos numéricos y de fecha sean válidos.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al actualizar la factura: " + ex.Message, "error");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MostrarMensaje("Campos limpiados.", "info");
            hdnFacturaId.Value = "";
            btnAgregar.Visible = true;
            btnActualizar.Visible = true;
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("cajero_facturas.aspx"); // Assuming this redirects to a list of facturas
        }

        private void LimpiarCampos()
        {
            txtIdFactura.Text = "Automático";
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd"); // Set to current date
            if (ddlCliente.Items.Count > 0) ddlCliente.SelectedIndex = 0;
            txtTotalFacturado.Text = "0.00";
            lblMensaje.Text = string.Empty;
            lblMensaje.CssClass = "";
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtFecha.Text) ||
                ddlCliente.SelectedValue == "" || // Validate that a client is selected
                string.IsNullOrWhiteSpace(txtTotalFacturado.Text))
            {
                MostrarMensaje("Por favor, complete todos los campos obligatorios.", "warning");
                return false;
            }

            if (!DateTime.TryParse(txtFecha.Text, out _))
            {
                MostrarMensaje("Formato de fecha inválido. Use AAAA-MM-DD.", "warning");
                return false;
            }

            if (!decimal.TryParse(txtTotalFacturado.Text, out decimal total) || total < 0)
            {
                MostrarMensaje("El total facturado debe ser un número válido (no negativo).", "warning");
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