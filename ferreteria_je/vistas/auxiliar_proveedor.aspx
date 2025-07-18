<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_proveedor.aspx.cs" Inherits="ferreteria_je.auxiliar_proveedor" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Proveedores - Auxiliar Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/cajero_ventas.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <%-- Asumo que tienes Font Awesome configurado, si no, reemplaza o añade la CDN --%>
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
    <%-- ¡Importante!: Se ha eliminado cualquier bloque <style> adicional aquí para no afectar tus estilos. --%>
</head>
<body>
    <form id="form1" runat="server">
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/>
                </div>
                <nav>
                    <ul>
                        <li><a href="auxiliar_proveedor.aspx" class="activo">Proveedores</a></li>
                        <li><a href="auxiliar_producto.aspx">Productos</a></li>
                        <li><a href="auxiliar_compra.aspx">Compras</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <div class="topbar">
                    <div class="user-menu">
                        <%-- Botón de cerrar sesión unificado con el ícono --%>
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
                    <h1>Gestión de Proveedores</h1>
                    <div class="actions">
                        <%-- Botón "Agregar proveedor": Sigue oculto para el auxiliar. --%>
                        <asp:Button ID="btnAddProveedor" runat="server" Text=" Agregar proveedor" CssClass="btn btn-primary"
                                     OnClientClick="window.location.href='auxiliar_gestion_proveedor.aspx'; return false;"
                                     Visible="false" />
                        
                        <%-- Campo de búsqueda y botón de búsqueda --%>
                        <asp:TextBox ID="txtBuscarProveedor" runat="server" CssClass="search-input" placeholder="Buscar proveedor..."></asp:TextBox>
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CssClass="btn btn-secondary" />

                        <%-- CAMBIO AQUÍ: Usamos asp:LinkButton en lugar de asp:Button para el botón de Excel --%>
                        <asp:LinkButton ID="btnExportarExcel" runat="server" OnClick="btnExportarExcel_Click" CssClass="btn btn-excel">
                            <i class="fas fa-file-excel"></i> Exportar a Excel
                        </asp:LinkButton>
                    </div>
                </div>

                <%-- Mensajes de feedback --%>
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="alert alert-info"></asp:Label>
                <asp:Label ID="lblErrorMessage" runat="server" EnableViewState="false" ForeColor="Red" CssClass="alert alert-danger"></asp:Label>


                <div class="table-container">
                    <%-- GridView para mostrar los datos de los proveedores --%>
                    <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False" CssClass="data-table"
                        AllowPaging="True" PageSize="10" OnPageIndexChanging="gvProveedores_PageIndexChanging"
                        EmptyDataText="No se encontraron proveedores." PagerStyle-CssClass="pager-style">
                        <Columns>
                            <%-- Mapping de columnas a las propiedades de tu modelo 'proveedor' --%>
                            <asp:BoundField DataField="id_proveedor" HeaderText="ID" SortExpression="id_proveedor" />
                            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                            <asp:BoundField DataField="telefono" HeaderText="Teléfono" SortExpression="telefono" />
                            <asp:BoundField DataField="direccion" HeaderText="Dirección" SortExpression="direccion" />
                            <asp:BoundField DataField="email" HeaderText="Correo" SortExpression="email" />

                            
                        </Columns>
                    </asp:GridView>
                </div>
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

            // Resaltar el enlace activo del sidebar
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
        </script>
    </form>
</body>
</html>