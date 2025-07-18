<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_producto.aspx.cs" Inherits="ferreteria_je.auxiliar_producto" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Productos - Auxiliar Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/auxiliar_proveedor.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
    <form id="form1" runat="server"> <%-- ¡IMPORTANTE! Envuelve todo el cuerpo dentro de un formulario ASP.NET --%>
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/>
                </div>
                <nav>
                    <ul>
                        <li><a href="auxiliar_proveedor.aspx">Proveedores</a></li>
                        <li><a href="auxiliar_producto.aspx" class="activo">Productos</a></li>
                        <li><a href="auxiliar_compra.aspx">Compras</a></li>
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
                    <h1>Gestión de Productos</h1>
                    <div class="actions">
                        <%-- Botón "Agregar producto": Visible para el auxiliar --%>
                        <asp:Button ID="btnAddProducto" runat="server" Text=" Agregar producto" CssClass="btn btn-primary"
                                     OnClientClick="window.location.href='auxiliar_gestion_producto.aspx'; return false;"
                                     Visible="true" />

                        <%-- Campo de búsqueda y botón de búsqueda --%>
                        <asp:TextBox ID="txtBuscarProducto" runat="server" CssClass="search-input" placeholder="Buscar producto..."></asp:TextBox>
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CssClass="btn btn-secondary" />

                        <%-- CAMBIO AQUÍ: Usamos asp:LinkButton en lugar de asp:Button para el botón de Exportar a Excel --%>
                        <asp:LinkButton ID="btnExportarExcel" runat="server" OnClick="btnExportarExcel_Click" CssClass="btn btn-excel">
                            <i class="fas fa-file-excel"></i> Exportar a Excel
                        </asp:LinkButton>
                    </div>
                </div>

                <%-- Mensajes de feedback --%>
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="alert alert-info"></asp:Label>
                <asp:Label ID="lblErrorMessage" runat="server" EnableViewState="false" ForeColor="Red" CssClass="alert alert-danger"></asp:Label>


                <div class="table-container">
                    <%-- GridView para mostrar los datos de los productos --%>
                    <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False" CssClass="data-table"
                        AllowPaging="True" PageSize="10" OnPageIndexChanging="gvProductos_PageIndexChanging"
                        EmptyDataText="No se encontraron productos." PagerStyle-CssClass="pager-style"
                        OnRowDataBound="gvProductos_RowDataBound"> <%-- Añadimos OnRowDataBound --%>
                        <Columns>
                            <%-- Mapping de columnas a las propiedades de tu modelo 'producto' --%>
                            <asp:BoundField DataField="id_producto" HeaderText="ID" SortExpression="id_producto" />
                            <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                            <asp:BoundField DataField="descripcion" HeaderText="Descripción" SortExpression="descripcion" />
                            <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" SortExpression="precio" />
                            <asp:BoundField DataField="stock" HeaderText="Stock" SortExpression="stock" />

                        </Columns>
                    </asp:GridView>
                </div>
            </main>
        </div>

        <script>
            // Función para alternar la visibilidad del menú desplegable del usuario
            function toggleDropdown() {
                const dropdown = document.getElementById('userDropdown');
                dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
            }

            // Ocultar el menú desplegable si se hace clic fuera de él
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
    </form> <%-- Cierre del formulario --%>
</body>
</html>