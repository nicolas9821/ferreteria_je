<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionusuarios.aspx.cs" Inherits="ferreteria_je.gestionusuarios" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Usuarios</title>
    <link rel="stylesheet" href="../estilos/styless.css" />
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
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
                <li><a href="inicio.aspx">Inicio</a></li>
                <li><a href="proveedores.aspx">Proveedores</a></li>
                <li><a href="productos.aspx">Productos</a></li>
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
        <h1>Gestión de Usuarios</h1>
        
        <div class="form-container"> <%-- Este es el contenedor clave para el estilo --%>
            <%-- Label para ID Usuario --%>
            <asp:Label ID="lblIdUsuario" runat="server" Text="ID Usuario:"></asp:Label>
            <asp:TextBox ID="txtIdUsuario" runat="server" CssClass="input-field" placeholder="ID Usuario (para Consultar/Actualizar/Eliminar)"></asp:TextBox>
            
            <%-- Label para Nombre --%>
            <asp:Label ID="lblNombre" runat="server" Text="Nombre:"></asp:Label>
            <asp:TextBox ID="txtNombre" runat="server" CssClass="input-field" placeholder="Nombre de usuario" required="required"></asp:TextBox>
            
            <%-- Label para Correo --%>
            <asp:Label ID="lblEmail" runat="server" Text="Correo:"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" placeholder="Correo" required="required"></asp:TextBox>
            
            <%-- Label para Contraseña --%>
            <asp:Label ID="lblPassword" runat="server" Text="Contraseña:"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="input-field" TextMode="Password" placeholder="Contraseña" required="required"></asp:TextBox>
            
            <%-- Label para Teléfono --%>
            <asp:Label ID="lblTelefono" runat="server" Text="Teléfono:"></asp:Label>
            <asp:TextBox ID="txtTelefono" runat="server" CssClass="input-field" placeholder="Teléfono"></asp:TextBox>
            
            <%-- Label para Rol --%>
            <asp:Label ID="lblRol" runat="server" Text="Rol:"></asp:Label>
            <asp:DropDownList ID="ddlRol" runat="server" CssClass="input-field">
                <asp:ListItem Text="Seleccionar Rol" Value="" />
                <asp:ListItem Text="Administrador" Value="admin" />
                <asp:ListItem Text="Cajero" Value="cajero" />
                <asp:ListItem Text="Auxiliar de Bodega" Value="bodega" />
            </asp:DropDownList>
        </div>
        
        <div class="acciones">
            <asp:Button ID="btnAgregar" runat="server" Text=" Agregar" CssClass="btn" OnClick="btnAgregar_Click" />
            <asp:Button ID="btnActualizar" runat="server" Text=" Actualizar" CssClass="btn" OnClick="btnActualizar_Click" />
            <asp:Button ID="btnEliminar" runat="server" Text=" Eliminar" CssClass="btn" OnClick="btnEliminar_Click" />
            <asp:Button ID="btnConsultar" runat="server" Text=" Consultar" CssClass="btn" OnClick="btnConsultar_Click" />
            <asp:Button ID="btnLimpiar" runat="server" Text=" Limpiar" CssClass="btn" OnClick="btnLimpiar_Click" CausesValidation="false" /> 
        </div>

        <%-- Etiqueta para mensajes al usuario --%>
        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

    </main>
</div>
</form>
</body>
</html>