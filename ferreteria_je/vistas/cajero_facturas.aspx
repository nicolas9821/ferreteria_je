<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cajero_facturas.aspx.cs" Inherits="ferreteria_je.cajero_facturas" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Facturas - Cajero Ferretería</title>
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/cajero_ventas.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
</head>
<body>
    <form id="form1" runat="server"> <%-- Agrega el control form de servidor --%>
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/>
                </div>
                <nav>
                    <ul>
                        <li><a href="cajero_productos.aspx">Productos</a></li>
                        <li><a href="cajero_ventas.aspx">Ventas</a></li>
                        <li><a href="cajero_facturas.aspx" class="activo">Facturas</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <div class="topbar">
                    <div class="user-menu">
                        <asp:Button ID="btnLogout" runat="server" Text=" Cerrar sesión" CssClass="logout-btn" OnClick="btnLogout_Click" /> <%-- ASP.NET Button --%>
                        <div class="user-name" onclick="toggleDropdown()">
                            <i class="fas fa-user-circle"></i> <asp:Literal ID="litUserName" runat="server"></asp:Literal> <%-- Mostrar nombre de usuario dinámico --%>
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
                    <h1>Gestión de Facturas</h1>
                    <div class="actions">
                        <button class="btn btn-primary" onclick="window.location.href='cajero_gestion_facturas.aspx'; return false;">
                             Gestionar factura
                        </button>
                        <%-- Convertir a TextBox y Button de ASP.NET --%>
                        <asp:TextBox ID="txtBuscarFactura" runat="server" CssClass="search-input" placeholder="Buscar factura por cliente o ID..."></asp:TextBox>
                        <asp:Button ID="btnBuscarFactura" runat="server" Text="Buscar" CssClass="btn btn-secondary" OnClick="btnBuscarFactura_Click" />
                        <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn btn-excel" OnClick="btnExportarExcel_Click" />
                    </div>
                </div>

                <div class="table-container">
    <asp:GridView ID="gvFacturas" runat="server" AutoGenerateColumns="False" 
        CssClass="data-table" HeaderStyle-CssClass="thead" RowStyle-CssClass="tbody-tr" 
        AlternatingRowStyle-CssClass="tbody-tr-alt" EmptyDataText="No hay facturas para mostrar."
        AllowPaging="True" PageSize="10" OnPageIndexChanging="gvFacturas_PageIndexChanging">
        <Columns>
            <asp:BoundField DataField="id_factura" HeaderText="ID" />
            <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" /> 
            <asp:BoundField DataField="total" HeaderText="Total" DataFormatString="{0:C}" />
            <%-- ELIMINADA LA COLUMNA DE ACCIONES --%>
        </Columns>
        <HeaderStyle CssClass="thead"></HeaderStyle>
        <RowStyle CssClass="tbody-tr"></RowStyle>
        <AlternatingRowStyle CssClass="tbody-tr-alt"></AlternatingRowStyle>
        <PagerStyle CssClass="gridview-pager" />
    </asp:GridView>
    <asp:Label ID="lblNoResults" runat="server" Text="No se encontraron facturas." Visible="false" CssClass="no-results-message"></asp:Label>
</div>
            </main>
        </div>
    </form> <%-- Cierra el control form de servidor --%>
    <script>
        // Función para mostrar/ocultar el menú desplegable
        function toggleDropdown() {
            const dropdown = document.getElementById('userDropdown');
            dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        }

        // Cerrar el menú si se hace clic fuera de él
        document.addEventListener('click', function (e) {
            const userMenu = document.querySelector('.user-menu');
            const dropdown = document.getElementById('userDropdown');

            // Si el clic no fue dentro del menú de usuario y el dropdown está abierto
            if (userMenu && !userMenu.contains(e.target) && dropdown.style.display === 'block') {
                dropdown.style.display = 'none';
            }
        });

        // NOTA: La función logout() ahora será manejada por el botón de servidor btnLogout_Click.
        // Si necesitas un logout vía JS para otro propósito, tendrías que hacer un PostBack o llamar a un WebMethod.

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
</body>
</html>