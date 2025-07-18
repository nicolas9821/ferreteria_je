<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionclientes.aspx.cs" Inherits="ferreteria_je.gestionclientes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Clientes</title>
    <link rel="stylesheet" href="../estilos/styless.css" />
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
<form id="formCliente" runat="server">
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
        <h1>Gestión de Clientes</h1>
        
        <div class="form-container">
            <%-- Label para Cédula de Búsqueda --%>
            <asp:Label ID="lblCedulaBusqueda" runat="server" Text="Cédula para búsqueda/actualización/eliminación:"></asp:Label>
            <%-- Campo para la cédula de búsqueda, actualización o eliminación (sin spinners) --%>
            <asp:TextBox ID="txtCedulaBusqueda" runat="server" Placeholder="Cédula del cliente (Actualizar/Eliminar)" TextMode="SingleLine" CssClass="input-field"></asp:TextBox>
            
            <%-- Label para Nombre del Cliente --%>
            <asp:Label ID="lblNombreCliente" runat="server" Text="Nombre del cliente:"></asp:Label>
            <%-- Campos para agregar o actualizar datos del cliente --%>
            <asp:TextBox ID="txtNombre" runat="server" Placeholder="Nombre del cliente" required="true" CssClass="input-field"></asp:TextBox>
            
            <%-- Label para Cédula (agregar/actualizar) --%>
            <asp:Label ID="lblCedula" runat="server" Text="Cédula:"></asp:Label>
            <%-- Campo de Cédula para agregar/actualizar (sin spinners) --%>
            <asp:TextBox ID="txtCedula" runat="server" Placeholder="Cédula" TextMode="SingleLine" required="true" CssClass="input-field"></asp:TextBox>
            
            <%-- Label para Dirección --%>
            <asp:Label ID="lblDireccion" runat="server" Text="Dirección:"></asp:Label>
            <asp:TextBox ID="txtDireccion" runat="server" Placeholder="Dirección" CssClass="input-field"></asp:TextBox>
            
            <%-- Label para Correo --%>
            <asp:Label ID="lblEmail" runat="server" Text="Correo:"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" Placeholder="Correo" CssClass="input-field"></asp:TextBox>
            
            <%-- Label para Teléfono --%>
            <asp:Label ID="lblTelefono" runat="server" Text="Teléfono:"></asp:Label>
            <asp:TextBox ID="txtTelefono" runat="server" Placeholder="Teléfono" CssClass="input-field"></asp:TextBox>
        </div>
        
        <div class="acciones">
            <asp:Button ID="btnAgregar" runat="server" Text=" Agregar" OnClick="btnAgregar_Click" CssClass="btn" />
            <asp:Button ID="btnActualizar" runat="server" Text=" Actualizar" OnClick="btnActualizar_Click" CssClass="btn" />
            <asp:Button ID="btnEliminar" runat="server" Text=" Eliminar" OnClick="btnEliminar_Click" CssClass="btn" />
            <asp:Button ID="btnConsultar" runat="server" Text=" Consultar" OnClick="btnConsultar_Click" CssClass="btn" />
        </div>

        <%-- Etiqueta para mostrar mensajes al usuario --%>
        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="message-label"></asp:Label>

        <hr/>
        
    </main>
</div>
</form>
</body>
</html>