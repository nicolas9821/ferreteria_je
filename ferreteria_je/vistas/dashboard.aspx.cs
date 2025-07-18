using System;
// using System.Web.UI.HtmlControls; // Ya no es necesario este using si no declaramos el control manualmente

namespace ferreteria_je
{
    public partial class dashboard : System.Web.UI.Page
    {
        // protected HtmlGenericControl currentDate; // ¡Elimina esta línea!

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Establece la fecha actual en el span con id "currentDate"
                // currentDate ya está disponible automáticamente por el runat="server" en el .aspx
                currentDate.InnerText = DateTime.Now.ToString("MMMM dd, yyyy"); // Ejemplo: Julio 05, 2025
            }
        }
    }
}