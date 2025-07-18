using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using ferreteria_je.Utilidades;

namespace ferreteria_je
{
    public partial class auxiliar_gestion_compra : ferreteria_je.session.BasePage
    {
        private ICompraRepository _compraRepository;
        private IProveedorRepository _proveedorRepository;

        public int? CurrentCompraId
        {
            get
            {
                if (ViewState["CurrentCompraId"] != null)
                    return (int)ViewState["CurrentCompraId"];
                return null;
            }
            set
            {
                ViewState["CurrentCompraId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _compraRepository = new CompraRepository();
            _proveedorRepository = new ProveedorRepository();

            if (!IsPostBack)
            {
                // Si vienes de una página de listado para EDITAR, podrías pasar el ID por QueryString
                if (Request.QueryString["id"] != null)
                {
                    if (int.TryParse(Request.QueryString["id"], out int idToLoad))
                    {
                        CurrentCompraId = idToLoad;
                        CargarDatosCompra(idToLoad);
                        // **CAMBIO CLAVE AQUÍ:** Habilitar Actualizar y deshabilitar Agregar cuando se carga un ID
                        btnActualizar.Visible = true; //
                        btnAgregar.Visible = false; //
                    }
                    else
                    {
                        MostrarMensaje("ID de compra inválido en la URL.", "error"); //
                        LimpiarCampos(); //
                        // Si el ID es inválido, volvemos al modo de agregar
                        btnActualizar.Visible = false; //
                        btnAgregar.Visible = true; //
                    }
                }
                else
                {
                    // Modo "Agregar" por defecto
                    LimpiarCampos(); //
                    btnActualizar.Visible = false; //
                    btnAgregar.Visible = true; //
                }
            }
        }

        private void CargarDatosCompra(int idCompra)
        {
            try
            {
                var compraExistente = _compraRepository.Get(c => c.id_compra == idCompra); //
                if (compraExistente != null) //
                {
                    txtProveedor.Text = _proveedorRepository.Get(p => p.id_proveedor == compraExistente.id_proveedor)?.nombre ?? "Proveedor Desconocido"; //
                    txtFechaCompra.Text = compraExistente.fecha.ToString("yyyy-MM-dd"); //
                    txtTotalCompra.Text = compraExistente.total.ToString(); //
                    MostrarMensaje($"Compra ID {idCompra} cargada exitosamente para edición.", "success"); //
                    CurrentCompraId = idCompra; //
                }
                else
                {
                    MostrarMensaje($"No se encontró ninguna compra con el ID: {idCompra}.", "warning"); //
                    LimpiarCampos(); //
                    CurrentCompraId = null; //
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la compra: {ex.Message}", "error"); //
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            CurrentCompraId = null; //

            if (!ValidarCampos()) //
            {
                return; //
            }

            try
            {
                var nuevaCompra = new compra(); //

                if (DateTime.TryParse(txtFechaCompra.Text, out DateTime fechaCompra)) //
                {
                    nuevaCompra.fecha = fechaCompra; //
                }
                else
                {
                    MostrarMensaje("La fecha de la compra no es válida. Use el formato AAAA-MM-DD.", "error"); //
                    return; //
                }

                int? idProveedor = null; //
                if (!string.IsNullOrWhiteSpace(txtProveedor.Text)) //
                {
                    var proveedor = _proveedorRepository.Get(p => p.nombre == txtProveedor.Text.Trim()); //
                    if (proveedor != null) //
                    {
                        nuevaCompra.id_proveedor = proveedor.id_proveedor; //
                    }
                    else
                    {
                        MostrarMensaje("El proveedor ingresado no existe. Por favor, verifique el nombre.", "error"); //
                        return; //
                    }
                }
                else
                {
                    nuevaCompra.id_proveedor = null; //
                }

                if (decimal.TryParse(txtTotalCompra.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal totalCompra)) //
                {
                    nuevaCompra.total = totalCompra; //
                }
                else
                {
                    MostrarMensaje("El total de la compra no es válido. Asegúrese de usar un formato numérico correcto (ej. 123.45).", "error"); //
                    return; //
                }

                _compraRepository.Add(nuevaCompra); //

                MostrarMensaje("¡Compra agregada exitosamente!", "success"); //
                LimpiarCampos(); //
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al agregar la compra: {ex.Message}", "error"); //
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (CurrentCompraId == null) //
            {
                MostrarMensaje("No hay una compra seleccionada para actualizar. Por favor, cargue una compra o agregue una nueva.", "warning"); //
                return; //
            }

            if (!ValidarCampos()) //
            {
                return; //
            }

            try
            {
                var compraToUpdate = _compraRepository.Get(c => c.id_compra == CurrentCompraId.Value); //

                if (compraToUpdate == null) //
                {
                    MostrarMensaje("La compra que intenta actualizar ya no existe.", "error"); //
                    LimpiarCampos(); //
                    CurrentCompraId = null; //
                    return; //
                }

                if (DateTime.TryParse(txtFechaCompra.Text, out DateTime fechaCompra)) //
                {
                    compraToUpdate.fecha = fechaCompra; //
                }
                else
                {
                    MostrarMensaje("La fecha de la compra no es válida. Use el formato AAAA-MM-DD.", "error"); //
                    return; //
                }

                int? idProveedor = null; //
                if (!string.IsNullOrWhiteSpace(txtProveedor.Text)) //
                {
                    var proveedor = _proveedorRepository.Get(p => p.nombre == txtProveedor.Text.Trim()); //
                    if (proveedor != null) //
                    {
                        idProveedor = proveedor.id_proveedor; //
                    }
                    else
                    {
                        MostrarMensaje("El proveedor ingresado no existe en la base de datos. Por favor, verifique el nombre.", "error"); //
                        return; //
                    }
                }
                compraToUpdate.id_proveedor = idProveedor; //

                if (decimal.TryParse(txtTotalCompra.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal totalCompra)) //
                {
                    compraToUpdate.total = totalCompra; //
                }
                else
                {
                    MostrarMensaje("El total de la compra no es válido. Asegúrese de usar un formato numérico correcto (ej. 123.45).", "error"); //
                    return; //
                }

                _compraRepository.Update(compraToUpdate); //

                MostrarMensaje("¡Compra actualizada exitosamente!", "success"); //
                LimpiarCampos(); //
                CurrentCompraId = null; //
                // Después de actualizar, regresamos al modo de agregar una nueva compra
                btnAgregar.Visible = true; //
                btnActualizar.Visible = false; //
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al actualizar la compra: {ex.Message}", "error"); //
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos(); //
            MostrarMensaje("Campos limpiados.", "info"); //
            CurrentCompraId = null; //
            btnAgregar.Visible = true; //
            btnActualizar.Visible = false; //
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("auxiliar_compra.aspx"); //
        }

        private void LimpiarCampos()
        {
            txtProveedor.Text = ""; //
            txtFechaCompra.Text = ""; //
            txtTotalCompra.Text = ""; //
            lblMensaje.Text = ""; //
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtProveedor.Text) || //
                string.IsNullOrWhiteSpace(txtFechaCompra.Text) || //
                string.IsNullOrWhiteSpace(txtTotalCompra.Text)) //
            {
                MostrarMensaje("Todos los campos son obligatorios.", "warning"); //
                return false; //
            }
            return true; //
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje; //
            lblMensaje.CssClass = "mensaje"; //
            switch (tipo) //
            {
                case "success": //
                    lblMensaje.CssClass += " success"; //
                    break;
                case "error": //
                    lblMensaje.CssClass += " error"; //
                    break;
                case "warning": //
                    lblMensaje.CssClass += " alert-warning"; //
                    break;
                case "info": //
                    lblMensaje.CssClass += " alert-info"; //
                    break;
                default: //
                    lblMensaje.CssClass += " alert-info"; //
                    break;
            }
        }
    }
}