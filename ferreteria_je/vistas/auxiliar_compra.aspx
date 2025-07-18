<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_compra.aspx.cs" Inherits="ferreteria_je.auxiliar_compra" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Compras - Auxiliar Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/auxiliar_proveedor.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head> 
<body>
    <%-- MUY IMPORTANTE: Asegúrate de que este form envuelva absolutamente TODO el contenido del <body> que interactúa con el servidor. --%>
    <form id="form1" runat="server">
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/>
                </div>
                <nav>
                    <ul>
                        <li><a href="auxiliar_proveedor.aspx">Proveedores</a></li>
                        <li><a href="auxiliar_producto.aspx">Productos</a></li>
                        <li><a href="auxiliar_compra.aspx" class="activo">Compras</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <div class="topbar">
                    <div class="user-menu">
                        <%-- CAMBIO AQUÍ: Usamos asp:LinkButton en lugar de asp:Button --%>
                        <asp:LinkButton ID="btnLogout" runat="server" CssClass="logout-btn btn btn-danger" OnClick="btnLogout_Click">
                            <i class="fas fa-sign-out-alt"></i> Cerrar sesión
                        </asp:LinkButton>

                        <div class="user-name" onclick="toggleDropdown()">
                            <i class="fas fa-user-circle"></i> <asp:Literal ID="litUserName" runat="server"></asp:Literal>
                        </div>
                        <div class="dropdown" id="userDropdown">
                            <div><strong>Nombre:</strong> <asp:Literal ID="litUserFullName" runat="server"></asp:Literal></div>
                            <div><strong>Correo:</strong> <asp:Literal ID="litUserEmail" runat="server"></asp:Literal></div>
                            <div><strong>Teléfono:</strong> <asp:Literal ID="litUserPhone" runat="server"></asp:Literal></div>
                            <div><a href="NUEVACONTRASEÑA.aspx">Cambiar contraseña</a></div>
                        </div>
                    </div>
                </div>

                <div class="header-section">
                    <h1>Gestión de Compras</h1>
                    <div class="actions">
                        <asp:LinkButton ID="btnNuevaCompra" runat="server" OnClick="btnNuevaCompra_Click" CssClass="btn btn-primary">
                              Nueva compra
                        </asp:LinkButton>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Buscar compra..." OnTextChanged="btnSearch_Click" AutoPostBack="true"></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="Buscar" OnClick="btnSearch_Click" CssClass="btn btn-secondary" />
                                               <%-- CAMBIO AQUÍ: Usamos asp:LinkButton en lugar de asp:Button para el botón de Exportar a Excel --%>
                        <asp:LinkButton ID="btnExportarExcel" runat="server" OnClick="btnExportarExcel_Click" CssClass="btn btn-excel">
                            <i class="fas fa-file-excel"></i> Exportar a Excel
                        </asp:LinkButton>
                    </div>
                </div>

                <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="alert alert-info"></asp:Label>
                <asp:Label ID="lblErrorMessage" runat="server" EnableViewState="false" ForeColor="Red" CssClass="alert alert-danger"></asp:Label>

                <div class="table-container">
                    <%-- GridView de Compras --%>
                    <asp:GridView ID="gvCompras" runat="server" AutoGenerateColumns="False"
                        CssClass="data-table"
                        HeaderStyle-CssClass="thead" RowStyle-CssClass="tbody-row" AlternatingRowStyle-CssClass="tbody-row-alt"
                        EmptyDataText="No hay compras para mostrar."
                        AllowPaging="True" PageSize="10" OnPageIndexChanging="gvCompras_PageIndexChanging"
                        DataKeyNames="id_compra"
                        OnRowEditing="gvCompras_RowEditing"
                        OnRowUpdating="gvCompras_RowUpdating"
                        OnRowCancelingEdit="gvCompras_RowCancelingEdit"
                        OnRowDeleting="gvCompras_RowDeleting">
                        <Columns>
                            <asp:BoundField DataField="id_compra" HeaderText="ID" SortExpression="id_compra" ReadOnly="True" />
                            <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:TemplateField HeaderText="Proveedor" SortExpression="id_proveedor">
                                <ItemTemplate>
                                    <asp:Label ID="lblProveedorName" runat="server" Text='<%# GetProveedorName(Eval("id_proveedor")) %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblProveedorNameEdit" runat="server" Text='<%# GetProveedorName(Eval("id_proveedor")) %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="total" HeaderText="Total" SortExpression="total" DataFormatString="{0:C}" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstado" runat="server" Text="Completada"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblEstadoEdit" runat="server" Text="Completada"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pager-style" HorizontalAlign="Center" />
                        <SortedAscendingHeaderStyle BackColor="#999999" />
                        <SortedDescendingHeaderStyle BackColor="#999999" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" />
                    </asp:GridView>
                </div>
            </main>
        </div>

        <script>
            function toggleDropdown() {
                const dropdown = document.getElementById('userDropdown');
                dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
            }

            document.addEventListener('click', function (event) {
                const userMenu = document.querySelector('.user-menu');
                const dropdown = document.getElementById('userDropdown');
                if (userMenu && !userMenu.contains(event.target) && dropdown.style.display === 'block') {
                    dropdown.style.display = 'none';
                }
            });

            document.addEventListener('DOMContentLoaded', function () {
                const currentPath = window.location.pathname.split('/').pop();
                const sidebarLinks = document.querySelectorAll('.sidebar nav ul li a');

                sidebarLinks.forEach(link => {
                    if (link.getAttribute('href').split('/').pop() === currentPath) {
                        link.classList.add('activo');
                    } else {
                        link.classList.remove('activo');
                    }
                });
            });

            function searchOnEnter(event) {
                if (event.keyCode === 13) {
                    document.getElementById('<%=btnSearch.ClientID%>').click();
                    return false;
                }
                return true;
            }
        </script>
    </form>
</body>
</html>