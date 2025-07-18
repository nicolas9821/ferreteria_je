using System;
using System.Web.UI;
using System.Web.UI.HtmlControls; // Necesario para el tipo HtmlGenericControl

namespace ferreteria_je
{
    public partial class quienes_somos1 : System.Web.UI.Page
    {
        // currentDate se generará automáticamente por el runat="server" en el .aspx
        protected HtmlGenericControl currentDate; // Se declara aquí para que Visual Studio lo reconozca en el diseñador.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Establece la fecha actual en el span con id "currentDate"
                currentDate.InnerText = DateTime.Now.ToString("MMMM dd, yyyy"); // Ejemplo: Julio 06, 2025
            }
        }
    }
}