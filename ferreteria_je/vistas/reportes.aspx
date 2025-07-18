<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reportes.aspx.cs" Inherits="ferreteria_je.vistas.reportes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Reportes - Administrador Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css">
    <link rel="stylesheet" href="../estilos/reportes.css"> <%-- ¡Mantén este enlace aquí! --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
    <div class="dashboard">
        <aside class="sidebar">
            <div style="text-align: center;">
                <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/>
            </div>
            <nav>
                <ul>
                    <li><a href="inicio.aspx">Inicio</a></li>
                    <li><a href="proveedores.aspx">Proveedores</a></li>
                    <li><a href="productos.aspx">Productos</a></li>
                    <li><a href="ventas.aspx">Ventas</a></li>
                    <li><a href="usuarios.aspx">Usuarios</a></li>
                    <li><a href="clientes.aspx">Clientes</a></li>
                    <li><a href="facturas.aspx">Facturas</a></li>
                    <li><a href="compra.aspx">Compras</a></li>
                    <li><a href="reportes.aspx" class="activo">Reportes</a></li>
                </ul>
            </nav>
        </aside>

        <main class="main-content">
            <form id="form1" runat="server"> <%-- El formulario ahora envuelve todo el contenido del main-content --%>
                <div class="topbar">
                    <div class="user-menu"> <%-- user-menu solo contiene los botones de usuario --%>
                        <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" CssClass="logout-btn">
                            <i class="fas fa-sign-out-alt"></i> Cerrar sesión
                        </asp:LinkButton>
                       
                        <div class="user-name" onclick="toggleDropdown()">
                            <i class="fas fa-user-circle"></i> Administrador
                        </div>
                        <div class="dropdown" id="userDropdown">
                            <div><strong>Nombre:</strong> Juan Pérez</div>
                            <div><strong>Correo:</strong> admin@ferreteria.com</div>
                            <div><strong>Teléfono:</strong> +52 123 456 7890</div>
                            <div><a href="NUEVACONTRASEÑA.aspx">Cambiar contraseña</a></div>
                        </div>
                    </div> <%-- Fin de user-menu --%>
                </div> <%-- Fin de topbar --%>
               
                <%-- Contenido principal de reportes, fuera del user-menu, pero dentro del form --%>
                <div class="report-content-centered">
                    <h1>Generar Reportes</h1>
                    <div class="report-buttons-container">
                        <asp:Button ID="btnReporteCompras" runat="server" Text=" Reporte de Compras" OnClick="btnReporteCompras_Click" CssClass="report-button" />
                        <asp:Button ID="btnReporteVentas" runat="server" Text=" Reporte de Ventas" OnClick="btnReporteVentas_Click" CssClass="report-button" />
                    </div>
                </div>
            </form> <%-- Fin del formulario --%>
        </main>
    </div>

    <script>
        // Tu JavaScript (permanece igual)
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
                if (link.getAttribute('href') === currentPath) {
                    link.classList.add('activo');
                } else {
                    link.classList.remove('activo');
                }
            });
        });
    </script>
</body>
</html>