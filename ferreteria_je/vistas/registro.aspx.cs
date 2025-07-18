using System;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models; // Ensure this points to your 'usuario' model
using ferreteria_je.Utilidades; // This is for your Seguridad.HashPassword() method

namespace ferreteria_je
{
    public partial class registro : System.Web.UI.Page
    {
        // Declare the repository at the class level
        private UsuarioRepository _usuarioRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the repository here
            _usuarioRepository = new UsuarioRepository();
            lblMensaje.Text = string.Empty; // Clear message on page load
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            // 1. Get values from ASP.NET controls and perform server-side validation
            string nombre = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string rol = ddlRol.SelectedValue; // Get the selected rol

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(rol)) // Validate rol selection
            {
                lblMensaje.Text = "Por favor, complete todos los campos, incluyendo el rol.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"\w+([-+.']\w+)@\w+([-.]\w+)\.\w+([-.]\w+)*"))
            {
                lblMensaje.Text = "Formato de correo electrónico inválido.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (password != confirmPassword)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Basic password complexity check (should match your client-side rules)
            if (password.Length < 8 || !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d") || !System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
            {
                lblMensaje.Text = "La contraseña debe tener al menos 8 caracteres, un número y un símbolo especial.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // 2. Check if email already exists in the database
                var existingUser = _usuarioRepository.GetByEmail(email);
                if (existingUser != null)
                {
                    lblMensaje.Text = "Este correo electrónico ya está registrado.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // 3. Hash the password before creating the user object
                string hashedPassword = Seguridad.HashPassword(password);

                // 4. Create new user object with hashed password and selected rol
                var newUser = new usuario
                {
                    nombre = nombre,
                    email = email,
                    password = hashedPassword, // Store the HASHED password here!
                    telefono = "", // Assuming phone is optional for registration. Adjust as needed.
                    rol = rol // Use the selected rol from the dropdown
                };

                // 5. Add user to the database via the repository
                _usuarioRepository.Add(newUser);

                lblMensaje.Text = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                lblMensaje.ForeColor = System.Drawing.Color.Green;

                // Optionally, clear the form fields after successful registration
                txtName.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtNewPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;
                ddlRol.SelectedValue = ""; // Reset the dropdown

                // Consider redirecting to login page after successful registration for better UX
                // Response.Redirect("login.aspx");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // If you have a Log utility: Log.Escribir($"Error en el registro de usuario: {ex.Message}", ex);
                lblMensaje.Text = "Ocurrió un error inesperado durante el registro. Por favor, inténtalo de nuevo.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}