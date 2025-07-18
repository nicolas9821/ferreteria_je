<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_gestion_compra.aspx.cs" Inherits="ferreteria_je.auxiliar_gestion_compra" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Compras</title>
    <link rel="stylesheet" href="../estilos/styless.css" />
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
                    <li><a href="auxiliar_proveedor.aspx">Proveedores</a></li>
                    <li><a href="auxiliar_producto.aspx">Productos</a></li>
                    <li><a href="auxiliar_compra.aspx">Compras</a></li>
                </ul>
            </nav>
        </aside>

        <main class="main-content">
            <h1>Gestión de Compras</h1>
            
            <div class="form-container">
                <form id="formCompra" runat="server">
                    <%-- Eliminado el campo ID de compra para gestionar, ahora se asume que esta vista es para Agregar/Actualizar --%>

                    <div class="form-group">
                        <label for="txtProveedor">Proveedor (Nombre):</label>
                        <asp:TextBox ID="txtProveedor" runat="server" CssClass="input-field" placeholder="Nombre del Proveedor" required></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtFechaCompra">Fecha de Compra:</label>
                        <asp:TextBox ID="txtFechaCompra" runat="server" CssClass="input-field" TextMode="Date" required></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtTotalCompra">Total de la Compra:</label>
                        <asp:TextBox ID="txtTotalCompra" runat="server" CssClass="input-field" TextMode="Number" placeholder="Total de la compra" step="0.01" required></asp:TextBox>
                    </div>
                    
                    <div class="acciones">
                        <%-- Botón Agregar --%>
                        <asp:LinkButton ID="btnAgregar" runat="server" OnClick="btnAgregar_Click" CssClass="btn btn-success">
                             Agregar
                        </asp:LinkButton>

                        <%-- Botón Actualizar --%>
                        <asp:LinkButton ID="btnActualizar" runat="server" OnClick="btnActualizar_Click" CssClass="btn btn-primary">
                            <i class="fas fa-pen"></i> Actualizar
                        </asp:LinkButton>

                        <%-- Botón Limpiar --%>
                        <asp:LinkButton ID="btnLimpiar" runat="server" OnClick="btnLimpiar_Click" CssClass="btn btn-secondary">
                            <i class="fas fa-sync-alt"></i> Limpiar
                        </asp:LinkButton>

                        <%-- Botón Volver --%>
                        <asp:LinkButton ID="btnVolver" runat="server" OnClick="btnVolver_Click" CssClass="btn btn-info">
                            <i class="fas fa-arrow-left"></i> Volver
                        </asp:LinkButton>
                    </div>
                    
                    <asp:Label ID="lblMensaje" runat="server" EnableViewState="false" CssClass="mensaje"></asp:Label>
                </form>
            </div>
        </main>
    </div>
</body>
</html>