<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionproductos.aspx.cs" Inherits="ferreteria_je.gestionproductos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Productos</title>
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
                <h1>Gestión de Productos</h1>
                
                <%-- Contenedor principal para los campos de entrada --%>
                <div class="form-container">
                    <asp:Label ID="lblIdProducto" runat="server" Text="ID Producto:"></asp:Label>
                    <asp:TextBox ID="txtIdProducto" runat="server" CssClass="input-field" placeholder="ID Producto (para actualizar/eliminar)" TextMode="Number"></asp:TextBox>
                    
                    <asp:Label ID="lblNombre" runat="server" Text="Nombre:"></asp:Label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="input-field" placeholder="Nombre del Producto" required="required"></asp:TextBox>
                    
                    <asp:Label ID="lblDescripcion" runat="server" Text="Descripción:"></asp:Label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="input-field" placeholder="Descripción"></asp:TextBox>
                    
                    <asp:Label ID="lblPrecio" runat="server" Text="Precio:"></asp:Label>
                    <asp:TextBox ID="txtPrecio" runat="server" CssClass="input-field" placeholder="Precio" TextMode="Number" required="required"></asp:TextBox>
                    
                    <asp:Label ID="lblStock" runat="server" Text="Stock:"></asp:Label>
                    <asp:TextBox ID="txtStock" runat="server" CssClass="input-field" placeholder="Stock" TextMode="Number" required="required"></asp:TextBox>
                </div>
                
                <%-- Contenedor para los botones de acción --%>
                <div class="acciones">
                    <asp:Button ID="btnAgregar" runat="server" Text=" Agregar" CssClass="btn" OnClick="btnAgregar_Click" />
                    <asp:Button ID="btnActualizar" runat="server" Text=" Actualizar" CssClass="btn" OnClick="btnActualizar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text=" Eliminar" CssClass="btn" OnClick="btnEliminar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" Text=" Limpiar" CssClass="btn" OnClick="btnLimpiar_Click" />
                </div>

                <%-- Etiqueta para mensajes al usuario --%>
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>
            </main>
        </div>
    </form>
</body>
</html>