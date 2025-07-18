using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using MySqlX.XDevAPI;
using System.Web.UI;
using ferreteria_je.Utilidades;

namespace ferreteria_je.session
{
    public class BasePage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Si no hay sesión activa, redirige al Login
            if (Session == null || Session.Keys.Count == 0)
            {
                Response.Redirect("~/vistas/login.aspx");
            }
        }
    }
}

