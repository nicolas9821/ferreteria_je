<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cajero_gestion_facturas.aspx.cs" Inherits="ferreteria_je.cajero_gestion_facturas" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Gestión de Facturas</title>
    <%-- Linking to the same CSS as gestion_ventas for consistent styling --%>
    <link rel="stylesheet" href="../estilos/auxiliar_producto.css" />
    <%-- Ensure Font Awesome is linked for icons --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <%-- The kit script is generally not needed if you link the CSS directly --%>
</head>
<body>
<div class="dashboard">
    <aside class="sidebar">
        <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
        <nav>
            <ul>
                <li><a href="cajero_productos.aspx">Productos</a></li>
                <li><a href="cajero_ventas.aspx">Ventas</a></li>
                <li><a href="cajero_facturas.aspx" class="activo">Facturas</a></li> <%-- 'activo' class for current page --%>
            </ul>
        </nav>
    </aside>

    <main class="main-content">
        <div class="form-container">
            <h1>Gestión de Facturas</h1>
            <form id="formFactura" runat="server">
                <%-- Hidden field to store the current factura ID for editing --%>
                <asp:HiddenField ID="hdnFacturaId" runat="server" Value="" />

                <%-- OCULTAR CAMPO ID FACTURA --%>
                <div class="form-group" style="display: none;">
                    <label for="txtIdFactura">ID Factura:</label>
                    <asp:TextBox ID="txtIdFactura" runat="server" CssClass="input-field" ReadOnly="true" placeholder="Automático"></asp:TextBox>
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
                    <label for="txtTotalFacturado">Total Facturado:</label>
                    <asp:TextBox ID="txtTotalFacturado" runat="server" CssClass="input-field" ReadOnly="True" placeholder="Calculado automáticamente" required="required"></asp:TextBox>
                </div>
                
                <%-- Sección de Botones (igual que en ventas) --%>
                <div class="acciones">
                    <asp:LinkButton ID="btnAgregar" runat="server" OnClick="btnAgregar_Click" CssClass="btn btn-primary">
                         Registrar Factura
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnActualizar" runat="server" OnClick="btnActualizar_Click" CssClass="btn btn-warning">
                        <i class="fas fa-pen"></i> Actualizar Factura
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
        // Set today's date if the field is empty on initial load
        if (fechaInput && !fechaInput.value) {
            const today = new Date();
            const year = today.getFullYear();
            const month = String(today.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
            const day = String(today.getDate()).padStart(2, '0');
            fechaInput.value = `${year}-${month}-${day}`;
        }
    });
</script>
</body>
</html>