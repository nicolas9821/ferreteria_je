<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionfacturas.aspx.cs" Inherits="ferreteria_je.gestionfacturas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Facturas</title>
    <link rel="stylesheet" href="../estilos/styless.css" /> 
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
    <form id="form1" runat="server">
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
                        <li><a href="facturas.aspx" class="activo">Facturas</a></li>
                        <li><a href="compra.aspx">Compras</a></li>
                        <li><a href="reportes.aspx">Reportes</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <h1>Gestión de Facturas</h1>
                
                <%-- Etiqueta para mensajes al usuario --%>
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

                <%-- Contenedor principal para los campos de entrada --%>
                <div class="form-container">
                    <%-- Label para ID Factura --%>
                    <asp:Label ID="lblIdFactura" runat="server" Text="ID Factura:"></asp:Label>
                    <asp:TextBox ID="txtIdFactura" runat="server" CssClass="input-field" placeholder="ID Factura (para actualizar/eliminar)" ReadOnly="True"></asp:TextBox>
                    
                    <%-- Label para Fecha --%>
                    <asp:Label ID="lblFecha" runat="server" Text="Fecha:"></asp:Label>
                    <asp:TextBox ID="txtFecha" runat="server" CssClass="input-field" TextMode="Date" placeholder="Fecha" required="required"></asp:TextBox>
                    
                    <%-- Label para Cliente --%>
                    <asp:Label ID="lblCliente" runat="server" Text="Cliente:"></asp:Label>
                    <%-- DropDownList para seleccionar el cliente (sin AutoPostBack ni OnSelectedIndexChanged) --%>
                    <asp:DropDownList ID="ddlCliente" runat="server" CssClass="input-field"></asp:DropDownList>

                    <%-- Label para Total --%>
                    <asp:Label ID="lblTotal" runat="server" Text="Total Facturado:"></asp:Label>
                    <asp:TextBox ID="txtTotal" runat="server" CssClass="input-field" placeholder="Total Facturado" TextMode="Number" required="required"></asp:TextBox>
                    
                    <%-- Placeholder para mantener el layout de 2 columnas si es necesario (mantengo el div vacío) --%>
                    <div></div> 
                </div>
                
                <%-- Contenedor para los botones de acción --%>
                <div class="acciones">
                    <asp:Button ID="btnAgregar" runat="server" Text="Agregar" CssClass="btn" OnClick="btnAgregar_Click" />
                    <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" CssClass="btn" OnClick="btnActualizar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btn" OnClick="btnEliminar_Click" OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta factura?');" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn" OnClick="btnLimpiar_Click" />
                </div>
            </main>
        </div>
    </form>
    <script>
        // Script para pre-seleccionar la fecha actual en el campo de fecha
        document.addEventListener('DOMContentLoaded', function () {
            const fechaInput = document.getElementById('<%= txtFecha.ClientID %>');
            if (fechaInput && !fechaInput.value) {
                const today = new Date();
                const year = today.getFullYear();
                const month = String(today.getMonth() + 1).padStart(2, '0'); // Meses son 0-index
                const day = String(today.getDate()).padStart(2, '0');
                fechaInput.value = `${year}-${month}-${day}`;
            }
        });
    </script>
</body>
</html>