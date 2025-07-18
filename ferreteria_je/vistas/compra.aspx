<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="compra.aspx.cs" Inherits="ferreteria_je.compras" Culture="es-CO" UICulture="es-CO" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server"> <%-- Añadir runat="server" aquí --%>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Compras - Administrador Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/proveedores.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
</head>
<body>
    <div class="dashboard">
        <aside class="sidebar">
            <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
            <nav>
                <ul>
                    <li><a href="inicio.aspx">Inicio</a></li>
                    <li><a href="proveedores.aspx">Proveedores</a></li>
                    <li><a href="productos.aspx">Productos</a></li>
                    <li><a href="ventas.aspx">Ventas</a></li>
                    <li><a href="usuarios.aspx">Usuarios</a></li>
                    <li><a href="clientes.aspx">Clientes</a></li>
                    <li><a href="facturas.aspx">Facturas</a></li>
                    <li><a href="compra.aspx" class="activo">Compras</a></li>
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
                            <%-- Estos literales se llenarán con los datos de sesión disponibles --%>
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
                        <asp:LinkButton ID="lnkNuevaCompra" runat="server" CssClass="btn btn-primary" OnClick="lnkNuevaCompra_Click">
                            Agregar Nueva Compra
                        </asp:LinkButton>
                         <asp:LinkButton ID="btnExportExcel" runat="server" CssClass="btn btn-secondary" OnClick="btnExportExcel_Click" ToolTip="Exportar a Excel">
                            <i class="fas fa-file-excel"></i> Exportar Excel
                        </asp:LinkButton>
                        <div class="search-container">
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="search-input" Placeholder="Buscar compra por ID o proveedor..." AutoPostBack="True" OnTextChanged="txtBuscar_TextChanged"></asp:TextBox>
                            <asp:LinkButton ID="btnBuscar" runat="server" CssClass="search-button" OnClick="btnBuscar_Click">
                                <i class="fa-solid fa-search"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="table-container">
                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="alert hidden"></asp:Label>
                    <asp:Label ID="lblErrorMessage" runat="server" EnableViewState="false" CssClass="alert alert-danger hidden"></asp:Label>

                    <asp:GridView ID="gvCompras" runat="server" AutoGenerateColumns="False"
                        CssClass="data-table" EmptyDataText="No hay compras registradas." AllowPaging="True" PageSize="10"
                        OnPageIndexChanging="gvCompras_PageIndexChanging" OnRowCommand="gvCompras_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="id_compra" HeaderText="ID Compra" SortExpression="id_compra" />
                            <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" SortExpression="fecha" />
                            <%-- Asumo que 'nombre_proveedor' sigue siendo válido --%>
                            <asp:BoundField DataField="nombre_proveedor" HeaderText="Proveedor" SortExpression="nombre_proveedor" />
                            <%-- Si 'nombre_usuario' no viene, esto quedará en blanco --%>
                            <asp:BoundField DataField="nombre_usuario" HeaderText="Registrado por" SortExpression="nombre_usuario" />
                            <asp:BoundField DataField="total" HeaderText="Total" DataFormatString="{0:C}" SortExpression="total" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="action-btn edit-btn" CommandName="EditarCompra" CommandArgument='<%# Eval("id_compra") %>'>
                                        <i class="fas fa-edit"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="action-btn delete-btn" CommandName="EliminarCompra" OnClientClick='return confirm("¿Estás seguro de que quieres eliminar esta compra?");' CommandArgument='<%# Eval("id_compra") %>'>
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
                const linkHref = link.getAttribute('href').split('/').pop().split('?')[0]; // Adjusting for query strings
                if (linkHref === currentPath) {
                    link.classList.add('activo');
                } else {
                    link.classList.remove('activo');
                }
            });
        });
    </script>
</body>
</html>