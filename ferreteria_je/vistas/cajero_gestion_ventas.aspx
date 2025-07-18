<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cajero_gestion_ventas.aspx.cs" Inherits="ferreteria_je.cajero_gestion_ventas" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Ventas</title>
    <link rel="stylesheet" href="../estilos/auxiliar_producto.css" /> 
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
</head>    
<body>
<div class="dashboard">
    <aside class="sidebar">
        <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
        <nav>
            <ul>
                <li><a href="cajero_productos.aspx">Productos</a></li>
                <li><a href="cajero_ventas.aspx" class="activo">Ventas</a></li>
                <li><a href="cajero_facturas.aspx">Facturas</a></li>
            </ul>
        </nav>
    </aside>

    <main class="main-content">
        <div class="form-container"> 
            <h1>Gestión de Ventas</h1> 
            
            <form id="formVenta" runat="server">
                <%-- Hidden field to store the current venta ID for editing --%>
                <asp:HiddenField ID="hdnVentaId" runat="server" Value="" />

                <%--
                    OCULTAMOS COMPLETAMENTE EL GRUPO DE FORMULARIO PARA ID VENTA
                    Este div contendrá tanto la label como el textbox
                --%>
                <div class="form-group" style="display: none;">
                    <label for="txtIdVenta">ID Venta:</label>
                    <asp:TextBox ID="txtIdVenta" runat="server" CssClass="input-field" ReadOnly="true" placeholder="Automático"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label for="txtFecha">Fecha:</label>
                    <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" CssClass="input-field" required="required"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label for="ddlCliente">Cliente:</label>
                    <asp:DropDownList ID="ddlCliente" runat="server" CssClass="input-field" required="required"></asp:DropDownList>
                </div>

                <div class="form-group">
                    <label for="ddlUsuario">Usuario:</label>
                    <asp:DropDownList ID="ddlUsuario" runat="server" CssClass="input-field" required="required"></asp:DropDownList>
                </div>

                <div class="form-group">
                    <label for="ddlProducto">Producto:</label>
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="input-field" AutoPostBack="True" OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged" required="required"></asp:DropDownList>
                </div>

                <div class="form-group">
                    <label for="txtCantidad">Cantidad:</label>
                    <asp:TextBox ID="txtCantidad" runat="server" TextMode="Number" CssClass="input-field" placeholder="Cantidad del producto" AutoPostBack="True" OnTextChanged="txtCantidad_TextChanged" required="required"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label for="txtPrecioUnitario">Precio Unitario:</label>
                    <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="input-field" ReadOnly="True" placeholder="Calculado automáticamente"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label for="txtTotalVenta">Total Venta:</label>
                    <asp:TextBox ID="txtTotalVenta" runat="server" CssClass="input-field" ReadOnly="True" placeholder="Calculado automáticamente"></asp:TextBox>
                </div>

                <%-- Sección de Botones --%>
                <div class="acciones">
                    <asp:LinkButton ID="btnAgregar" runat="server" OnClick="btnAgregar_Click" CssClass="btn btn-primary">
                         Registrar Venta
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnActualizar" runat="server" OnClick="btnActualizar_Click" CssClass="btn btn-warning">
                        <i class="fas fa-pen"></i> Actualizar Venta
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnLimpiar" runat="server" OnClick="btnLimpiar_Click" CssClass="btn btn-secondary">
                         Limpiar
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

<script>
    document.addEventListener('DOMContentLoaded', function () {
        const fechaInput = document.getElementById('<%= txtFecha.ClientID %>');
        if (fechaInput && !fechaInput.value) {
            const today = new Date();
            const year = today.getFullYear();
            const month = String(today.getMonth() + 1).padStart(2, '0');
            const day = String(today.getDate()).padStart(2, '0');
            fechaInput.value = `${year}-${month}-${day}`;
        }
    });
</script>
</body>
</html>