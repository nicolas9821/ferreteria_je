
using System;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;

namespace ferreteria_je
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            var sesionRepo = new SesionRepository();
            var usuario = sesionRepo.ValidarCredenciales(email, password);

            if (usuario != null)
            {
                // Guardar usuario y rol en sesión
                Session["usuario"] = usuario;
                Session["UserRole"] = usuario.rol; // <--- ¡Importante cambio!

                string userRole = usuario.rol.ToLower(); // Convert to lowercase for consistent comparison

                switch (userRole)
                {
                    case "admin":
                        Response.Redirect("inicio.aspx");
                        break;
                    case "auxiliar":
                        Response.Redirect("auxiliar_proveedor.aspx");
                        break;
                    case "cajero":
                        Response.Redirect("cajero_productos.aspx");
                        break;
                    case "cliente":
                        Response.Redirect("cliente.aspx");
                        break;
                    default:
                        lblMensaje.Text = "Rol no reconocido.";
                        lblMensaje.Visible = true;
                        break;
                }
            }
            else
            {
                lblMensaje.Text = "Correo o contraseña incorrectos.";
                lblMensaje.Visible = true;
            }
        }
    }
}
