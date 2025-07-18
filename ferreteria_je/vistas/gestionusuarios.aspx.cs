using System;
using System.Configuration;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades; // Eliminado: Ya no se necesita Seguridad.cs
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ferreteria_je
{
    public partial class gestionusuarios : ferreteria_je.session.BasePage
    {
        private UsuarioRepository _usuarioRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            _usuarioRepository = new UsuarioRepository();

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    int idUsuario;
                    if (int.TryParse(Request.QueryString["id"], out idUsuario))
                    {
                        txtIdUsuario.Text = idUsuario.ToString();
                        CargarDatosUsuario(idUsuario);
                        ConfigurarModoEdicion();
                    }
                    else
                    {
                        // Log.Escribir("ID de usuario inválido en la URL: " + Request.QueryString["id"]); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                        lblMensaje.Text = "ID de usuario inválido en la URL.";
                        ConfigurarModoAgregar();
                    }
                }
                else
                {
                    ConfigurarModoAgregar();
                }
            }
        }

        private void ConfigurarModoAgregar()
        {
            btnAgregar.Visible = true;
            btnActualizar.Visible = false;
            btnEliminar.Visible = false;
            btnConsultar.Visible = true;
            txtIdUsuario.ReadOnly = false;
            LimpiarCampos();
            lblMensaje.Text = "Ingrese los datos para un nuevo usuario o el ID para consultar.";

            txtPassword.Attributes["required"] = "required";
            txtPassword.Attributes["placeholder"] = "Contraseña";
        }

        private void ConfigurarModoEdicion()
        {
            btnAgregar.Visible = false;
            btnActualizar.Visible = true;
            btnEliminar.Visible = true;
            btnConsultar.Visible = true;
            txtIdUsuario.ReadOnly = true;
            lblMensaje.Text = "Usuario encontrado. Puede modificarlo y hacer clic en Actualizar, o eliminar.";

            txtPassword.Attributes.Remove("required");
            txtPassword.Attributes["placeholder"] = "Deja en blanco para mantener contraseña";
        }

        private void CargarDatosUsuario(int idUsuario)
        {
            try
            {
                var usuarioEncontrado = _usuarioRepository.Get(u => u.id_usuario == idUsuario);

                if (usuarioEncontrado != null)
                {
                    txtIdUsuario.Text = usuarioEncontrado.id_usuario.ToString();
                    txtNombre.Text = usuarioEncontrado.nombre;
                    txtEmail.Text = usuarioEncontrado.email;
                    txtTelefono.Text = usuarioEncontrado.telefono;
                    ddlRol.SelectedValue = usuarioEncontrado.rol;
                    lblMensaje.Text = "Usuario encontrado. Puede modificarlo y hacer clic en Actualizar.";
                    ConfigurarModoEdicion();
                }
                else
                {
                    // Log.Escribir($"Usuario con ID {idUsuario} no encontrado."); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                    lblMensaje.Text = $"Usuario con ID {idUsuario} no encontrado.";
                    LimpiarCampos();
                    ConfigurarModoAgregar();
                }
            }
            catch (Exception ex)
            {
                // Log.Escribir($"Error al cargar datos del usuario con ID {idUsuario}: {ex.Message}", ex); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                lblMensaje.Text = "Error al cargar el usuario. Verifique los logs.";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(ddlRol.SelectedValue) || ddlRol.SelectedValue == "")
            {
                lblMensaje.Text = "Por favor, complete todos los campos obligatorios (Nombre, Correo, Contraseña, Rol).";
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                lblMensaje.Text = "Formato de correo electrónico inválido.";
                return;
            }

            try
            {
                var existingUser = _usuarioRepository.GetByEmail(txtEmail.Text.Trim());
                if (existingUser != null)
                {
                    lblMensaje.Text = "Este correo electrónico ya está registrado.";
                    return;
                }

                var nuevoUsuario = new usuario
                {
                    nombre = txtNombre.Text.Trim(),
                    telefono = txtTelefono.Text.Trim(),
                    email = txtEmail.Text.Trim(),
                    password = txtPassword.Text.Trim(), // <-- ¡Cambiado aquí! Ahora se guarda en texto plano
                    rol = ddlRol.SelectedValue
                };

                _usuarioRepository.Add(nuevoUsuario);
                lblMensaje.Text = "Usuario agregado correctamente.";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
            catch (Exception ex)
            {
                // Log.Escribir("Error al agregar usuario: " + ex.Message, ex); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                lblMensaje.Text = "Error al agregar usuario. Consulte los logs.";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(ddlRol.SelectedValue) || ddlRol.SelectedValue == "")
            {
                lblMensaje.Text = "Para actualizar, ingrese un ID y complete todos los campos obligatorios (Nombre, Correo, Rol).";
                return;
            }

            int idUsuario;
            if (!int.TryParse(txtIdUsuario.Text, out idUsuario))
            {
                lblMensaje.Text = "ID de usuario inválido para actualizar.";
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                lblMensaje.Text = "Formato de correo electrónico inválido.";
                return;
            }

            try
            {
                var existingUserWithEmail = _usuarioRepository.GetByEmail(txtEmail.Text.Trim());
                if (existingUserWithEmail != null && existingUserWithEmail.id_usuario != idUsuario)
                {
                    lblMensaje.Text = "Este correo electrónico ya está registrado para otro usuario.";
                    return;
                }

                var usuarioModificado = new usuario
                {
                    id_usuario = idUsuario,
                    nombre = txtNombre.Text.Trim(),
                    telefono = txtTelefono.Text.Trim(),
                    email = txtEmail.Text.Trim(),
                    rol = ddlRol.SelectedValue
                };

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    usuarioModificado.password = txtPassword.Text.Trim(); // <-- ¡Cambiado aquí! Ahora se guarda en texto plano
                }
                else
                {
                    var usuarioExistente = _usuarioRepository.Get(u => u.id_usuario == idUsuario);
                    if (usuarioExistente != null)
                    {
                        usuarioModificado.password = usuarioExistente.password;
                    }
                    else
                    {
                        // Log.Escribir($"Error: No se pudo recuperar la contraseña existente para el usuario con ID {idUsuario}."); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                        lblMensaje.Text = "Error: No se pudo recuperar la contraseña existente para el usuario.";
                        return;
                    }
                }

                _usuarioRepository.Update(usuarioModificado);
                lblMensaje.Text = "Usuario actualizado correctamente.";
                CargarDatosUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                // Log.Escribir($"Error al actualizar usuario con ID {idUsuario}: {ex.Message}", ex); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                lblMensaje.Text = "Error al actualizar usuario. Consulte los logs.";
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdUsuario.Text))
            {
                lblMensaje.Text = "Ingrese el ID del usuario a eliminar.";
                return;
            }

            int idUsuario;
            if (!int.TryParse(txtIdUsuario.Text, out idUsuario))
            {
                lblMensaje.Text = "ID de usuario inválido para eliminar.";
                return;
            }

            try
            {
                var usuarioAEliminar = new usuario { id_usuario = idUsuario };

                _usuarioRepository.Delete(usuarioAEliminar);
                lblMensaje.Text = "Usuario eliminado correctamente.";
                LimpiarCampos();
                ConfigurarModoAgregar();
            }
            catch (Exception ex)
            {
                // Log.Escribir($"Error al eliminar usuario con ID {idUsuario}: {ex.Message}", ex); // Si Log.Escribir depende de Utilidades, también deberías eliminarlo o adaptarlo
                lblMensaje.Text = "Error al eliminar usuario. Consulte los logs.";
            }
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdUsuario.Text))
            {
                lblMensaje.Text = "Ingrese el ID del usuario a consultar.";
                return;
            }

            int idUsuario;
            if (!int.TryParse(txtIdUsuario.Text, out idUsuario))
            {
                lblMensaje.Text = "ID de usuario inválido para consultar.";
                return;
            }

            CargarDatosUsuario(idUsuario);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarModoAgregar();
        }

        private void LimpiarCampos()
        {
            txtIdUsuario.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            ddlRol.SelectedValue = string.Empty;
            lblMensaje.Text = string.Empty;
        }
    }
}
