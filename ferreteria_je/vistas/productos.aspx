<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="productos.aspx.cs" Inherits="ferreteria_je.productos" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Productos - Administrador Ferretería</title>
    
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/proveedores.css"> <%-- Reutilizamos el CSS de proveedores --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script> <%-- Para los iconos --%>
</head>
<body>
    <div class="dashboard">
        <aside class="sidebar">
            <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
            <nav>
                <ul>
                    <li><a href="inicio.aspx">Inicio</a></li>
                    <li><a href="proveedores.aspx">Proveedores</a></li>
                    <li><a href="productos.aspx" class="activo">Productos</a></li>
                    <li><a href="ventas.aspx">Ventas</a></li>
                    <li><a href="usuarios.aspx">Usuarios</a></li>
                    <li><a href="clientes.aspx">Clientes</a></li>
                    <li><a href="facturas.aspx">Facturas</a></li>
                    <li><a href="compra.aspx">Compras</a></li>
                    <li><a href="reportes.aspx">Reportes</a></li>
                </ul>
            </nav>
        </aside>

        <main class="main-content">
            <form id="form1" runat="server">
                <div class="topbar">
                    <div class="user-menu">
                        <asp:LinkButton ID="lnkCerrarSesion" runat="server" CssClass="logout-btn" OnClick="lnkCerrarSesion_Click">
                                <i class="fas fa-sign-out-alt"></i> Cerrar sesión
                        </asp:LinkButton>
                        <div class="user-name" onclick="toggleDropdown()">
                                <i class="fas fa-user-circle"></i> <asp:Literal ID="litUserNameButton" runat="server"></asp:Literal>
                        </div>
                        <div class="dropdown" id="userDropdown">
                            <%-- Literales para mostrar los detalles del usuario --%>
                            <div><strong>Nombre:</strong> <asp:Literal ID="litUserFullName" runat="server"></asp:Literal></div>
                            <div><strong>Correo:</strong> <asp:Literal ID="litUserEmail" runat="server"></asp:Literal></div>
                            <div><strong>Teléfono:</strong> <asp:Literal ID="litUserPhone" runat="server"></asp:Literal></div>
                            <div><a href="NUEVACONTRASEÑA.aspx">Cambiar contraseña</a></div>
                        </div>
                    </div>
                </div>

                <div class="header-section">
                    <h1>Gestión de Productos</h1>
                    <div class="actions">
                        <asp:LinkButton ID="lnkAgregarProducto" runat="server" CssClass="btn btn-primary" OnClick="lnkAgregarProducto_Click">
                                Agregar producto
                        </asp:LinkButton>
                        
                        <%-- BOTÓN DE EXPORTAR A EXCEL --%>
                        <asp:LinkButton ID="btnExportExcel" runat="server" CssClass="btn btn-secondary" OnClick="btnExportExcel_Click" ToolTip="Exportar a Excel">
                            <i class="fas fa-file-excel"></i> Exportar Excel
                        </asp:LinkButton>

                        <div class="search-container">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Buscar producto..."></asp:TextBox>
                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-button" OnClick="btnSearch_Click">
                                    <i class="fa-solid fa-search"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="table-container">
                    <%-- Mensajes de estado y error --%>
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert hidden"></asp:Label>
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert alert-danger hidden"></asp:Label>

                   
<asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False"
    CssClass="data-table" EmptyDataText="No hay productos registrados." AllowPaging="True" PageSize="10"
    OnPageIndexChanging="gvProductos_PageIndexChanging" OnRowCommand="gvProductos_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="id_producto" HeaderText="ID" Visible="False" SortExpression="id_producto" /> <%-- Añadido SortExpression --%>
                            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" /> <%-- Añadido SortExpression --%>
                            <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" /> <%-- Añadido SortExpression --%>
                            <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" SortExpression="precio" /> <%-- Añadido SortExpression --%>
                            <asp:BoundField DataField="stock" HeaderText="Stock" SortExpression="stock" /> <%-- Añadido SortExpression --%>
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="action-btn edit-btn" CommandName="EditarProducto" CommandArgument='<%# Eval("id_producto") %>'>
                                        <i class="fas fa-edit"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="action-btn delete-btn" CommandName="EliminarProducto" OnClientClick='return confirm("¿Estás seguro de que quieres eliminar este producto?");' CommandArgument='<%# Eval("id_producto") %>'>
                                        <i class="fas fa-trash-alt"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#333" ForeColor="#fff" Font-Bold="True" />
                        <RowStyle BackColor="#ffffff" />
                        <AlternatingRowStyle BackColor="#f9f9f9" />
                        <PagerStyle BackColor="#e2e2e2" ForeColor="#333333" HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </form>
        </main>
    </div>

    <script>
        function toggleDropdown() {
            const dropdown = document.getElementById('userDropdown');
            dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        }

        document.addEventListener('click', function (e) {
            const menu = document.querySelector('.user-menu');
            if (menu && !menu.contains(e.target)) {
                const dropdown = document.getElementById('userDropdown');
                if (dropdown) {
                    dropdown.style.display = 'none';
                }
            }
        });

        document.addEventListener('DOMContentLoaded', function () {
            const currentPath = window.location.pathname.split('/').pop();
            const sidebarLinks = document.querySelectorAll('.sidebar nav ul li a');

            sidebarLinks.forEach(link => {
                if (link.getAttribute('href') && link.getAttribute('href').endsWith(currentPath)) {
                    link.classList.add('activo');
                } else {
                    link.classList.remove('activo');
                }
            });
        });
    </script>
</body>
</html>