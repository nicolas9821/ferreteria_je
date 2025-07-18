using ferreteria_je.Repositories;
using ferreteria_je.Repositories.Interfaces;
using ferreteria_je.Repositories.Models;
using ferreteria_je.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // <<<< Asegúrate que esta línea esté presente
using System.Web.UI;
using System.Web.UI.WebControls;

// Necesitamos importar el repositorio de proveedores si queremos obtener sus nombres
using ferreteria_je.Repositories.RepositoriesGeneric; // Para GenericRepository
using ferreteria_je.Repositories.RepositoriesGeneric.Interfaces; // Para IRepository


namespace ferreteria_je
{
    public partial class auxiliar_compra : ferreteria_je.session.BasePage
    {
        private CompraRepository _compraRepository;
        private IRepository<proveedor> _proveedorRepository;
        private IUsuarioRepository _usuarioRepository;

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

        private static List<proveedor> _cachedProveedores;

        protected void Page_Init(object sender, EventArgs e)
        {
            _compraRepository = new CompraRepository();
            _proveedorRepository = new GenericRepository<proveedor>();
            _usuarioRepository = new UsuarioRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || (Session["UserRole"].ToString().ToLower() != "auxiliar" && Session["UserRole"].ToString().ToLower() != "administrador"))
                {
                    Response.Redirect("~/vistas/login.aspx");
                    return;
                }

                CargarDatosUsuarioEnInterfaz();
                LoadProveedoresCache();
                BindGridView();
            }
            CheckUserRolePermissions();
        }

        private void CargarDatosUsuarioEnInterfaz()
        {
            if (Session["usuario"] is usuario currentUser)
            {
                litUserName.Text = currentUser.nombre;
                litUserFullName.Text = currentUser.nombre;
                litUserEmail.Text = currentUser.email;
                litUserPhone.Text = currentUser.telefono;
            }
            else
            {
                Response.Redirect("~/vistas/login.aspx");
            }
        }

        private void LoadProveedoresCache()
        {
            try
            {
                if (_cachedProveedores == null)
                {
                    _cachedProveedores = _proveedorRepository.GetAll().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar el caché de proveedores: {ex.Message}", ex);
                lblErrorMessage.Text = $"Error al cargar la información de proveedores: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger visible";
            }
        }

        private void CheckUserRolePermissions()
        {
            if (Session["UserRole"] != null)
            {
                string currentUserRoleName = Session["UserRole"].ToString().ToLower();
                btnNuevaCompra.Visible = (currentUserRoleName == "auxiliar" || currentUserRoleName == "administrador");
            }
            else
            {
                btnNuevaCompra.Visible = false;
            }
        }

        protected string GetProveedorName(object idProveedorObj)
        {
            if (idProveedorObj == DBNull.Value || idProveedorObj == null)
            {
                return "N/A";
            }

            int idProveedor = Convert.ToInt32(idProveedorObj);
            var proveedor = _cachedProveedores?.FirstOrDefault(p => p.id_proveedor == idProveedor);

            return proveedor != null ? proveedor.nombre : "Desconocido";
        }

        private void BindGridView()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                // Siempre empezar con una lista para asegurar que las operaciones LINQ son correctas.
                List<compra> compras = _compraRepository.GetAll().ToList();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    compras = compras.Where(c =>
                        c.id_compra.ToString().Contains(searchTerm) ||
                        c.fecha.ToString("yyyy-MM-dd").Contains(searchTerm) ||
                        GetProveedorName(c.id_proveedor).ToLower().Contains(searchTerm.ToLower())
                    ).ToList(); // Asegurar que es una lista después del filtro
                }

                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    // Almacenar el resultado ordenado en una variable temporal y luego reasignar.
                    // O hacer el ToList() inmediatamente.
                    // Para simplificar, convertimos el IEnumerable a List de nuevo
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_compra":
                                compras = compras.OrderBy(c => c.id_compra).ToList();
                                break;
                            case "fecha":
                                compras = compras.OrderBy(c => c.fecha).ToList();
                                break;
                            case "id_proveedor":
                                compras = compras.OrderBy(c => c.id_proveedor).ToList();
                                break;
                            case "total":
                                compras = compras.OrderBy(c => c.total).ToList();
                                break;
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_compra":
                                compras = compras.OrderByDescending(c => c.id_compra).ToList();
                                break;
                            case "fecha":
                                compras = compras.OrderByDescending(c => c.fecha).ToList();
                                break;
                            case "id_proveedor":
                                compras = compras.OrderByDescending(c => c.id_proveedor).ToList();
                                break;
                            case "total":
                                compras = compras.OrderByDescending(c => c.total).ToList();
                                break;
                        }
                    }
                }

                if (compras != null && compras.Any())
                {
                    gvCompras.DataSource = compras;
                    gvCompras.DataBind();
                    lblMessage.Text = $"Se han cargado {compras.Count()} compras.";
                    lblMessage.CssClass = "alert alert-info visible";
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden";
                }
                else
                {
                    gvCompras.DataSource = null;
                    gvCompras.DataBind();
                    lblMessage.Text = "No se encontraron compras.";
                    lblMessage.CssClass = "alert alert-info visible";
                    lblErrorMessage.Text = string.Empty;
                    lblErrorMessage.CssClass = "alert alert-danger hidden";
                }
            }
            catch (Exception ex)
            {
                Log.Escribir($"Error al cargar las compras: {ex.Message}", ex);
                lblErrorMessage.Text = "Ocurrió un error al cargar las compras. Por favor, intente de nuevo más tarde.";
                lblErrorMessage.CssClass = "alert alert-danger visible";
                lblMessage.Text = string.Empty;
                lblMessage.CssClass = "alert alert-info hidden";
            }
        }

        protected void gvCompras_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompras.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        protected void gvCompras_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGridView();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvCompras.PageIndex = 0;
            BindGridView();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                // Usar List<compra> desde el inicio para consistencia
                List<compra> comprasToExport = _compraRepository.GetAll().ToList();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    comprasToExport = comprasToExport.Where(c =>
                        c.id_compra.ToString().Contains(searchTerm) ||
                        c.fecha.ToString("yyyy-MM-dd").Contains(searchTerm) ||
                        GetProveedorName(c.id_proveedor).ToLower().Contains(searchTerm.ToLower())
                    ).ToList(); // Asegurar que es una lista después del filtro
                }

                if (!string.IsNullOrWhiteSpace(_sortExpression))
                {
                    // Asegúrate de que las propiedades a ordenar existan en tu clase 'compra'
                    // Convertir a lista después de la ordenación
                    if (_sortDirection == "ASC")
                    {
                        switch (_sortExpression)
                        {
                            case "id_compra":
                                comprasToExport = comprasToExport.OrderBy(c => c.id_compra).ToList();
                                break;
                            case "fecha":
                                comprasToExport = comprasToExport.OrderBy(c => c.fecha).ToList();
                                break;
                            case "id_proveedor":
                                comprasToExport = comprasToExport.OrderBy(c => c.id_proveedor).ToList();
                                break;
                            case "total":
                                comprasToExport = comprasToExport.OrderBy(c => c.total).ToList();
                                break;
                        }
                    }
                    else // DESC
                    {
                        switch (_sortExpression)
                        {
                            case "id_compra":
                                comprasToExport = comprasToExport.OrderByDescending(c => c.id_compra).ToList();
                                break;
                            case "fecha":
                                comprasToExport = comprasToExport.OrderByDescending(c => c.fecha).ToList();
                                break;
                            case "id_proveedor":
                                comprasToExport = comprasToExport.OrderByDescending(c => c.id_proveedor).ToList();
                                break;
                            case "total":
                                comprasToExport = comprasToExport.OrderByDescending(c => c.total).ToList();
                                break;
                        }
                    }
                }

                GridView gvExport = new GridView();
                gvExport.DataSource = comprasToExport;

                gvExport.Columns.Add(new BoundField { DataField = "id_compra", HeaderText = "ID" });
                gvExport.Columns.Add(new BoundField { DataField = "fecha", HeaderText = "Fecha", DataFormatString = "{0:yyyy-MM-dd}" });

                TemplateField tfProveedor = new TemplateField { HeaderText = "Proveedor" };
                tfProveedor.ItemTemplate = new ExportProveedorNameTemplate(ListItemType.Item, this);
                gvExport.Columns.Add(tfProveedor);

                gvExport.Columns.Add(new BoundField { DataField = "total", HeaderText = "Total", DataFormatString = "{0:C}" });

                TemplateField tfEstado = new TemplateField { HeaderText = "Estado" };
                tfEstado.ItemTemplate = new ExportEstadoTemplate(ListItemType.Item);
                gvExport.Columns.Add(tfEstado);

                gvExport.DataBind();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename=Compras_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls");
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

        private class ExportProveedorNameTemplate : ITemplate
        {
            private ListItemType templateType;
            private auxiliar_compra _parentPage;

            public ExportProveedorNameTemplate(ListItemType type, auxiliar_compra parentPage)
            {
                templateType = type;
                _parentPage = parentPage;
            }

            public void InstantiateIn(Control container)
            {
                if (templateType == ListItemType.Item || templateType == ListItemType.AlternatingItem)
                {
                    Literal lit = new Literal();
                    lit.DataBinding += (sender, e) => {
                        Literal l = (Literal)sender;
                        GridViewRow row = (GridViewRow)l.NamingContainer;
                        object idProveedorObj = DataBinder.Eval(row.DataItem, "id_proveedor");
                        l.Text = _parentPage.GetProveedorName(idProveedorObj);
                    };
                    container.Controls.Add(lit);
                }
            }
        }

        private class ExportEstadoTemplate : ITemplate
        {
            private ListItemType templateType;

            public ExportEstadoTemplate(ListItemType type)
            {
                templateType = type;
            }

            public void InstantiateIn(Control container)
            {
                if (templateType == ListItemType.Item || templateType == ListItemType.AlternatingItem)
                {
                    Literal lit = new Literal();
                    lit.Text = "Completada";
                    container.Controls.Add(lit);
                }
            }
        }

        protected void btnNuevaCompra_Click(object sender, EventArgs e)
        {
            Response.Redirect("auxiliar_gestion_compra.aspx");
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Este método DEBE estar vacío.
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vistas/login.aspx");
        }

        protected void gvCompras_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('La edición no está habilitada en esta vista. Use el formulario de gestión si lo necesita.');", true);
        }

        protected void gvCompras_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('La actualización no está habilitada en esta vista.');", true);
        }

        protected void gvCompras_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCompras.EditIndex = -1;
            BindGridView();
        }

        protected void gvCompras_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('La eliminación no está habilitada en esta vista. Si desea anular una compra, registre una devolución.');", true);
        }
    }
}