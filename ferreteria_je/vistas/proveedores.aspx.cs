using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using ferreteria_je.session;
using MySql.Data.MySqlClient; // ¡¡¡Esta línea debe estar presente para MySQL!!!

namespace ferreteria_je
{
    public partial class proveedores : ferreteria_je.session.BasePage
    {
        private ProveedorRepository _proveedorRepository;

        private string _sortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private string _sortExpression
        {
            get { return ViewState["SortExpression"] as string ?? ""; }
            set { ViewState["SortExpression"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _proveedorRepository = new ProveedorRepository();

            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString().ToLower() != "admin")
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarDatosUsuarioEnInterfaz();
                CargarProveedores();
                ClearMessages(); // Limpiar mensajes en la carga inicial
            }
        }

        private void ClearMessages()
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "alert alert-info hidden";
            lblErrorMessage.Text = string.Empty;
            lblErrorMessage.CssClass = "alert alert-danger hidden";
        }

        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserNameButton.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        private void CargarProveedores()
        {
            List<proveedor> listaProveedores = new List<proveedor>();
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                listaProveedores = _proveedorRepository.GetAll().ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    listaProveedores = listaProveedores
                        .Where(p => p.nombre.ToLower().Contains(searchTerm.ToLower()) ||
                                     p.direccion.ToLower().Contains(searchTerm.ToLower()) ||
                                     (p.telefono != null && p.telefono.Contains(searchTerm)) ||
                                     (p.email != null && p.email.ToLower().Contains(searchTerm.ToLower())))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "nombre": listaProveedores = listaProveedores.OrderBy(p => p.nombre).ToList(); break;
                            case "direccion": listaProveedores = listaProveedores.OrderBy(p => p.direccion).ToList(); break;
                            case "telefono": listaProveedores = listaProveedores.OrderBy(p => p.telefono).ToList(); break;
                            case "email": listaProveedores = listaProveedores.OrderBy(p => p.email).ToList(); break;
                            default: listaProveedores = listaProveedores.OrderBy(p => p.id_proveedor).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "nombre": listaProveedores = listaProveedores.OrderByDescending(p => p.nombre).ToList(); break;
                            case "direccion": listaProveedores = listaProveedores.OrderByDescending(p => p.direccion).ToList(); break;
                            case "telefono": listaProveedores = listaProveedores.OrderByDescending(p => p.telefono).ToList(); break;
                            case "email": listaProveedores = listaProveedores.OrderByDescending(p => p.email).ToList(); break;
                            default: listaProveedores = listaProveedores.OrderByDescending(p => p.id_proveedor).ToList(); break;
                        }
                    }
                }
                else
                {
                    listaProveedores = listaProveedores.OrderBy(p => p.id_proveedor).ToList();
                }

                gvProveedores.DataSource = listaProveedores;
                gvProveedores.DataBind();

                // ¡¡¡AJUSTE CLAVE AQUÍ!!!
                // Solo actualiza los mensajes de éxito si no hay un mensaje de error activo.
                if (string.IsNullOrEmpty(lblErrorMessage.Text)) // Si no hay un error establecido por EliminarProveedor
                {
                    if (listaProveedores != null && listaProveedores.Any())
                    {
                        lblMessage.Text = $"Se han cargado {listaProveedores.Count()} proveedores.";
                        lblMessage.CssClass = "alert alert-info visible";
                    }
                    else
                    {
                        lblMessage.Text = "No se encontraron proveedores.";
                        lblMessage.CssClass = "alert alert-info visible";
                    }
                    lblErrorMessage.Text = string.Empty; // Asegura que el error esté oculto
                    lblErrorMessage.CssClass = "alert alert-danger hidden";
                }
                // Si lblErrorMessage.Text NO está vacío, significa que EliminarProveedor ya puso un error,
                // y CargarProveedores no debe sobrescribirlo con el mensaje de éxito.
            }
            catch (Exception ex)
            {
                Log.Escribir("Error al cargar proveedores en la página: " + ex.Message, ex);
                lblErrorMessage.Text = "No se pudieron cargar los proveedores. Intente de nuevo más tarde.";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;
            CargarProveedores();
        }

        protected void gvProveedores_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (_sortExpression == e.SortExpression)
            {
                _sortDirection = (_sortDirection == "ASC" ? "DESC" : "ASC");
            }
            else
            {
                _sortExpression = e.SortExpression;
                _sortDirection = "ASC";
            }
            CargarProveedores();
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditarProveedor")
            {
                int idProveedor = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"gestionproveedores.aspx?id={idProveedor}");
            }
            else if (e.CommandName == "EliminarProveedor")
            {
                int idProveedor = Convert.ToInt32(e.CommandArgument);
                EliminarProveedor(idProveedor);
                // SIEMPRE recarga los datos después de una acción que los modifica.
                // El ajuste en CargarProveedores ahora evitará que sobrescriba el mensaje de error.
                CargarProveedores();
            }
        }

        private void EliminarProveedor(int idProveedor)
        {
            ClearMessages(); // Siempre limpia los mensajes anteriores antes de intentar una nueva acción

            try
            {
                var proveedorAEliminar = new proveedor { id_proveedor = idProveedor };
                _proveedorRepository.Delete(proveedorAEliminar);

                lblMessage.Text = $"Proveedor con ID {idProveedor} eliminado correctamente.";
                lblMessage.CssClass = "alert alert-success visible";
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al eliminar proveedor con ID {idProveedor}: {ex.Message}", ex);

                string userFriendlyErrorMessage = "Error al eliminar el proveedor. Intente de nuevo más tarde.";

                Exception currentEx = ex;
                while (currentEx != null)
                {
                    if (currentEx is MySqlException mySqlEx)
                    {
                        if (mySqlEx.Number == 1451) // MySQL error code for foreign key constraint violation
                        {
                            userFriendlyErrorMessage = $"No se puede eliminar el proveedor con ID {idProveedor} porque tiene compras asociadas. Elimine las compras relacionadas primero.";
                            break;
                        }
                    }
                    currentEx = currentEx.InnerException;
                }

                lblErrorMessage.Text = userFriendlyErrorMessage;
                lblErrorMessage.CssClass = "alert alert-danger visible"; // Asegura que el mensaje de error sea visible
            }
        }

        protected void lnkAgregarProveedor_Click(object sender, EventArgs e)
        {
            Response.Redirect("gestionproveedores.aspx");
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = 0;
            CargarProveedores();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                List<proveedor> proveedoresToExport = _proveedorRepository.GetAll().ToList();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    proveedoresToExport = proveedoresToExport
                        .Where(p => p.nombre.ToLower().Contains(searchTerm.ToLower()) ||
                                     p.direccion.ToLower().Contains(searchTerm.ToLower()) ||
                                     (p.telefono != null && p.telefono.Contains(searchTerm)) ||
                                     (p.email != null && p.email.ToLower().Contains(searchTerm.ToLower())))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "nombre": proveedoresToExport = proveedoresToExport.OrderBy(p => p.nombre).ToList(); break;
                            case "direccion": proveedoresToExport = proveedoresToExport.OrderBy(p => p.direccion).ToList(); break;
                            case "telefono": proveedoresToExport = proveedoresToExport.OrderBy(p => p.telefono).ToList(); break;
                            case "email": proveedoresToExport = proveedoresToExport.OrderBy(p => p.email).ToList(); break;
                            default: proveedoresToExport = proveedoresToExport.OrderBy(p => p.id_proveedor).ToList(); break;
                        }
                    }
                    else
                    {
                        switch (_sortExpression)
                        {
                            case "nombre": proveedoresToExport = proveedoresToExport.OrderByDescending(p => p.nombre).ToList(); break;
                            case "direccion": proveedoresToExport = proveedoresToExport.OrderByDescending(p => p.direccion).ToList(); break;
                            case "telefono": proveedoresToExport = proveedoresToExport.OrderByDescending(p => p.telefono).ToList(); break;
                            case "email": proveedoresToExport = proveedoresToExport.OrderByDescending(p => p.email).ToList(); break;
                            default: proveedoresToExport = proveedoresToExport.OrderByDescending(p => p.id_proveedor).ToList(); break;
                        }
                    }
                }
                else
                {
                    proveedoresToExport = proveedoresToExport.OrderBy(p => p.id_proveedor).ToList();
                }

                GridView gvExport = new GridView();
                gvExport.DataSource = proveedoresToExport;

                gvExport.Columns.Add(new BoundField { DataField = "id_proveedor", HeaderText = "ID" });
                gvExport.Columns.Add(new BoundField { DataField = "nombre", HeaderText = "Nombre" });
                gvExport.Columns.Add(new BoundField { DataField = "direccion", HeaderText = "Dirección" });
                gvExport.Columns.Add(new BoundField { DataField = "telefono", HeaderText = "Teléfono" });
                gvExport.Columns.Add(new BoundField { DataField = "email", HeaderText = "Correo" });

                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Proveedores_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        gvExport.RenderControl(hw);
                        Response.Write(sw.ToString());
                    }
                }

                Response.Flush();
                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Esta excepción es normal cuando se usa Response.End()
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al exportar a Excel: {ex.Message}", ex);
                lblErrorMessage.Text = $"Ocurrió un error al exportar los datos: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }
    }
}
