using System;

using System.Collections.Generic;

using System.Linq;

using System.Web.UI;

using System.Web.UI.WebControls;

using ferreteria_je.Repositories;

using ferreteria_je.Repositories.Models;

using ferreteria_je.Utilidades;



namespace ferreteria_je

{

    public partial class gestioncompras : ferreteria_je.session.BasePage

    {

        private CompraRepository _compraRepository;

        private ProveedorRepository _proveedorRepository; // Necesario para cargar proveedores

        private UsuarioRepository _usuarioRepository;     // Necesario para cargar usuarios (re-habilitado)



        protected void Page_Load(object sender, EventArgs e)

        {

            _compraRepository = new CompraRepository();

            _proveedorRepository = new ProveedorRepository();

            _usuarioRepository = new UsuarioRepository(); // Instanciar el repositorio de usuario



            if (!IsPostBack)

            {

                CargarProveedores(); // Cargar los proveedores en el DropDownList

                CargarUsuarios();   // Cargar los usuarios en el DropDownList



                if (Request.QueryString["id"] != null)

                {

                    int idCompra;

                    if (int.TryParse(Request.QueryString["id"], out idCompra))

                    {

                        CargarDatosCompra(idCompra);

                        ConfigurarModoEdicion();

                    }

                    else

                    {

                        lblMensaje.Text = "ID de compra inválido en la URL.";

                        ConfigurarModoAgregar();

                    }

                }

                else

                {

                    ConfigurarModoAgregar();

                }

            }

        }



        /// <summary>

        /// Carga los datos de los proveedores en el DropDownList.

        /// </summary>

        private void CargarProveedores()

        {

            try

            {

                var proveedores = _proveedorRepository.GetAll().ToList();

                ddlProveedor.DataSource = proveedores;

                ddlProveedor.DataTextField = "nombre"; // Campo a mostrar en el DropDownList (ajusta si es diferente)

                ddlProveedor.DataValueField = "id_proveedor"; // Valor asociado a cada ítem (ajusta si es diferente)

                ddlProveedor.DataBind();

                ddlProveedor.Items.Insert(0, new ListItem("-- Seleccione un Proveedor --", "")); // Opción por defecto

            }

            catch (Exception ex)

            {

                Log.Escribir("Error al cargar proveedores para el DropDownList: " + ex.Message, ex);

                lblMensaje.Text = "Error al cargar proveedores. Consulte los logs.";

            }

        }



        /// <summary>

        /// Carga los datos de los usuarios en el DropDownList.

        /// </summary>

        private void CargarUsuarios()

        {

            try

            {

                var usuarios = _usuarioRepository.GetAll().ToList();

                ddlUsuario.DataSource = usuarios;

                ddlUsuario.DataTextField = "nombre"; // <--- ¡CAMBIO AQUÍ!

                ddlUsuario.DataValueField = "id_usuario";

                ddlUsuario.DataBind();

                ddlUsuario.Items.Insert(0, new ListItem("-- Seleccione un Usuario --", ""));

            }

            catch (Exception ex)

            {

                Log.Escribir("Error al cargar usuarios para el DropDownList: " + ex.Message, ex);

                // lblMensaje.Text = "Error al cargar usuarios. Consulte los logs."; // Si tienes un label para mensajes

            }

        }





        private void ConfigurarModoAgregar()

        {

            btnAgregar.Visible = true;

            btnActualizar.Visible = false;

            btnEliminar.Visible = false;

            txtIdCompra.ReadOnly = true;

            LimpiarCampos();

            lblMensaje.Text = "Ingrese los datos para una nueva compra.";

        }



        private void ConfigurarModoEdicion()

        {

            btnAgregar.Visible = false;

            btnActualizar.Visible = true;

            btnEliminar.Visible = true;

            txtIdCompra.ReadOnly = true;

        }



        private void CargarDatosCompra(int idCompra)

        {

            try

            {

                var compraEncontrada = _compraRepository.Get(c => c.id_compra == idCompra);



                if (compraEncontrada != null)

                {

                    txtIdCompra.Text = compraEncontrada.id_compra.ToString();

                    txtFecha.Text = compraEncontrada.fecha.ToString("yyyy-MM-dd");

                    txtTotal.Text = compraEncontrada.total.ToString();



                    // Seleccionar el proveedor en el DropDownList

                    if (compraEncontrada.id_proveedor.HasValue)

                    {

                        ListItem itemProveedor = ddlProveedor.Items.FindByValue(compraEncontrada.id_proveedor.Value.ToString());

                        if (itemProveedor != null)

                        {

                            ddlProveedor.SelectedValue = compraEncontrada.id_proveedor.Value.ToString();

                        }

                        else

                        {

                            lblMensaje.Text += " (Proveedor asociado no encontrado o inactivo)";

                            ddlProveedor.SelectedValue = ""; // Seleccionar opción por defecto

                        }

                    }

                    else

                    {

                        ddlProveedor.SelectedValue = ""; // Si no hay proveedor asociado

                    }



                    // Seleccionar el usuario en el DropDownList

                    if (compraEncontrada.id_usuario.HasValue)

                    {

                        ListItem itemUsuario = ddlUsuario.Items.FindByValue(compraEncontrada.id_usuario.Value.ToString());

                        if (itemUsuario != null)

                        {

                            ddlUsuario.SelectedValue = compraEncontrada.id_usuario.Value.ToString();

                        }

                        else

                        {

                            lblMensaje.Text += " (Usuario asociado no encontrado o inactivo)";

                            ddlUsuario.SelectedValue = ""; // Seleccionar opción por defecto

                        }

                    }

                    else

                    {

                        ddlUsuario.SelectedValue = ""; // Si no hay usuario asociado

                    }



                    lblMensaje.Text = "Compra encontrada. Puede modificarla y hacer clic en Actualizar.";

                }

                else

                {

                    lblMensaje.Text = $"Compra con ID {idCompra} no encontrada. Creando una nueva.";

                    ConfigurarModoAgregar();

                }

            }

            catch (Exception ex)

            {

                Log.Escribir($"Error al cargar datos de la compra con ID {idCompra}: {ex.Message}", ex);

                lblMensaje.Text = "Error al cargar la compra. Verifique los logs para más detalles.";

                ConfigurarModoAgregar();

            }

        }



        protected void btnAgregar_Click(object sender, EventArgs e)

        {

            // Validar campos requeridos y que los DropDownList tengan una selección válida

            if (!Page.IsValid || string.IsNullOrWhiteSpace(txtFecha.Text) || string.IsNullOrWhiteSpace(txtTotal.Text))

            {

                lblMensaje.Text = "Por favor, complete todos los campos obligatorios y haga una selección válida.";

                return;

            }



            try

            {

                DateTime fechaCompra;

                if (!DateTime.TryParse(txtFecha.Text, out fechaCompra))

                {

                    lblMensaje.Text = "Formato de fecha inválido.";

                    return;

                }



                decimal totalCompra;

                if (!decimal.TryParse(txtTotal.Text, out totalCompra) || totalCompra < 0)

                {

                    lblMensaje.Text = "Total inválido. Debe ser un número positivo.";

                    return;

                }



                // Obtener IDs de DropDownLists (ya validados por RequiredFieldValidator)

                int idProveedor = int.Parse(ddlProveedor.SelectedValue);

                int idUsuario = int.Parse(ddlUsuario.SelectedValue); // Obtener el ID del DDL de Usuario



                var nuevaCompra = new compra

                {

                    fecha = fechaCompra,

                    id_proveedor = idProveedor,

                    id_usuario = idUsuario,

                    total = totalCompra

                };



                _compraRepository.Add(nuevaCompra);

                lblMensaje.Text = "Compra agregada correctamente.";

                LimpiarCampos();

            }

            catch (Exception ex)

            {

                Log.Escribir("Error al agregar compra: " + ex.Message, ex);

                lblMensaje.Text = "Error al agregar compra. Consulte los logs.";

            }

        }



        protected void btnActualizar_Click(object sender, EventArgs e)

        {

            // Validar que el ID de la compra y los campos obligatorios no estén vacíos

            if (!Page.IsValid || string.IsNullOrWhiteSpace(txtIdCompra.Text) ||

        string.IsNullOrWhiteSpace(txtFecha.Text) || string.IsNullOrWhiteSpace(txtTotal.Text))

            {

                lblMensaje.Text = "Para actualizar, ingrese el ID de la compra, complete todos los campos obligatorios y haga una selección válida.";

                return;

            }



            int idCompra;

            if (!int.TryParse(txtIdCompra.Text, out idCompra))

            {

                lblMensaje.Text = "ID de compra inválido para actualizar.";

                return;

            }



            try

            {

                DateTime fechaCompra;

                if (!DateTime.TryParse(txtFecha.Text, out fechaCompra))

                {

                    lblMensaje.Text = "Formato de fecha inválido.";

                    return;

                }



                decimal totalCompra;

                if (!decimal.TryParse(txtTotal.Text, out totalCompra) || totalCompra < 0)

                {

                    lblMensaje.Text = "Total inválido. Debe ser un número positivo.";

                    return;

                }



                // Obtener IDs de DropDownLists (ya validados por RequiredFieldValidator)

                int idProveedor = int.Parse(ddlProveedor.SelectedValue);

                int idUsuario = int.Parse(ddlUsuario.SelectedValue); // Obtener el ID del DDL de Usuario



                var compraModificada = new compra

                {

                    id_compra = idCompra,

                    fecha = fechaCompra,

                    id_proveedor = idProveedor,

                    id_usuario = idUsuario,

                    total = totalCompra

                };



                _compraRepository.Update(compraModificada);

                lblMensaje.Text = "Compra actualizada correctamente.";

            }

            catch (Exception ex)

            {

                Log.Escribir($"Error al actualizar compra con ID {idCompra}: {ex.Message}", ex);

                lblMensaje.Text = "Error al actualizar compra. Consulte los logs.";

            }

        }



        protected void btnEliminar_Click(object sender, EventArgs e)

        {

            if (string.IsNullOrWhiteSpace(txtIdCompra.Text))

            {

                lblMensaje.Text = "Ingrese el ID de la compra a eliminar.";

                return;

            }



            int idCompra;

            if (!int.TryParse(txtIdCompra.Text, out idCompra))

            {

                lblMensaje.Text = "ID de compra inválido para eliminar.";

                return;

            }



            try

            {

                var compraAEliminar = new compra { id_compra = idCompra };

                _compraRepository.Delete(compraAEliminar);

                lblMensaje.Text = "Compra eliminada correctamente.";

                LimpiarCampos();

                ConfigurarModoAgregar();

            }

            catch (Exception ex)

            {

                Log.Escribir($"Error al eliminar compra con ID {idCompra}: {ex.Message}", ex);

                lblMensaje.Text = "Error al eliminar compra. Consulte los logs.";

            }

        }



        protected void btnLimpiar_Click(object sender, EventArgs e)

        {

            LimpiarCampos();

            ConfigurarModoAgregar();

        }



        private void LimpiarCampos()

        {

            txtIdCompra.Text = string.Empty;

            txtFecha.Text = string.Empty;

            ddlProveedor.SelectedValue = ""; // Restablecer DropDownList a la opción por defecto

            ddlUsuario.SelectedValue = "";   // Restablecer DropDownList a la opción por defecto

            txtTotal.Text = string.Empty;

            lblMensaje.Text = string.Empty;

        }

    }

}