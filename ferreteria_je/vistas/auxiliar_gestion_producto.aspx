<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_gestion_producto.aspx.cs" Inherits="ferreteria_je.auxiliar_gestion_producto" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Productos</title>
    <link rel="stylesheet" href="../estilos/auxiliar_producto.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <%-- The Font Awesome kit script below is often redundant if you're using the CSS link above
         and just need the core icons. If you need advanced features from the kit, keep it.
         For basic icons, the CSS link is usually sufficient. --%>
    <%-- <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script> --%>
</head>    
<body>
<div class="dashboard">
    <aside class="sidebar">
        <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
        <nav>
            <ul>
                <li><a href="auxiliar_proveedor.aspx">Proveedores</a></li>
                <li><a href="auxiliar_producto.aspx" class="activo">Productos</a></li> <%-- 'activo' class indicates the current page --%>
                <li><a href="auxiliar_compra.aspx">Compras</a></li>
            </ul>
        </nav>
    </aside>

    <main class="main-content">
        <%-- Agregamos un div para el contenedor del formulario para aplicar estilos como el sombreado --%>
        <div class="form-container"> <%-- Nuevo contenedor para el formulario --%>
            <h1>Gestión de Productos</h1> 
            
            <form id="formProducto" runat="server">
                <%-- Campos de entrada --%>
                <div class="form-group">
                    <label for="txtNombreProducto">Nombre del Producto:</label>
                    <asp:TextBox ID="txtNombreProducto" runat="server" CssClass="input-field" placeholder="Nombre del producto" required="required"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtDescripcion">Descripción:</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="input-field" placeholder="Descripción del producto"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtPrecio">Precio:</label>
                    <asp:TextBox ID="txtPrecio" runat="server" TextMode="Number" CssClass="input-field" placeholder="Precio (ej. 10.50)" required="required"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtStock">Stock:</label>
                    <asp:TextBox ID="txtStock" runat="server" TextMode="Number" CssClass="input-field" placeholder="Stock (cantidad)" required="required"></asp:TextBox>
                </div>

                <%-- Sección de Botones --%>
                <div class="acciones">
                    <asp:LinkButton ID="btnAgregar" runat="server" OnClick="btnAgregar_Click" CssClass="btn btn-primary">
                         Agregar
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnLimpiar" runat="server" OnClick="btnLimpiar_Click" CssClass="btn btn-secondary">
                         Limpiar
                    </asp:LinkButton>
                    
                    <%-- BOTÓN ACTUALIZAR AÑADIDO AQUÍ, junto a Volver --%>
                    <asp:LinkButton ID="btnActualizar" runat="server" OnClick="btnActualizar_Click" CssClass="btn btn-warning">
                        <i class="fas fa-pen"></i> Actualizar
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnVolver" runat="server" OnClick="btnVolver_Click" CssClass="btn btn-info">
                        Volver
                    </asp:LinkButton>
                </div>
                
                <%-- Etiqueta para mostrar mensajes al usuario --%>
                <asp:Label ID="lblMensaje" runat="server" EnableViewState="false" CssClass="mensaje"></asp:Label>
            </form>
        </div>
    </main>
</div>
</body>
</html>