<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestioncompras.aspx.cs" Inherits="ferreteria_je.gestioncompras" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Compras</title>
    <link rel="stylesheet" href="../estilos/styless.css" />
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria" />
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
                <h1>Gestión de Compras</h1>

                <%-- Etiqueta para mensajes al usuario --%>
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

                <%-- Contenedor principal para los campos de entrada --%>
                <div class="form-container">
                    <%-- Label para ID Compra --%>
                    <asp:Label ID="lblIdCompra" runat="server" Text="ID Compra:"></asp:Label>
                    <asp:TextBox ID="txtIdCompra" runat="server" CssClass="input-field" placeholder="ID Compra (para Actualizar/Eliminar)" ReadOnly="True"></asp:TextBox>
                    
                    <%-- Label para Fecha de Compra --%>
                    <asp:Label ID="lblFecha" runat="server" Text="Fecha de Compra:"></asp:Label>
                    <asp:TextBox ID="txtFecha" runat="server" CssClass="input-field" TextMode="Date" placeholder="Fecha de Compra" required="required"></asp:TextBox>
                    
                    <%-- Label para Proveedor --%>
                    <asp:Label ID="lblProveedor" runat="server" Text="Proveedor:"></asp:Label>
                    <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="input-field"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvProveedor" runat="server" ControlToValidate="ddlProveedor" 
                                               InitialValue="" ErrorMessage="Seleccione un proveedor." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>

                    <%-- Label para Usuario --%>
                    <asp:Label ID="lblUsuario" runat="server" Text="Usuario:"></asp:Label>
                    <asp:DropDownList ID="ddlUsuario" runat="server" CssClass="input-field"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvUsuario" runat="server" ControlToValidate="ddlUsuario" 
                                               InitialValue="" ErrorMessage="Seleccione un usuario." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>

                    <%-- Label para Total de la Compra --%>
                    <asp:Label ID="lblTotal" runat="server" Text="Total de la Compra:"></asp:Label>
                    <asp:TextBox ID="txtTotal" runat="server" CssClass="input-field" placeholder="Total de la Compra" required="required"></asp:TextBox>
                    <asp:CompareValidator ID="cvTotal" runat="server" ControlToValidate="txtTotal" Type="Currency" 
                                          Operator="GreaterThanEqual" ValueToCompare="0" ErrorMessage="El total debe ser un número positivo." 
                                          ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>

                <%-- Contenedor para los botones de acción --%>
                <div class="acciones">
                    <asp:Button ID="btnAgregar" runat="server" Text=" Agregar" CssClass="btn" OnClick="btnAgregar_Click" />
                    <asp:Button ID="btnActualizar" runat="server" Text=" Actualizar" CssClass="btn" OnClick="btnActualizar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text=" Eliminar" CssClass="btn" OnClick="btnEliminar_Click" OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta compra?');" />
                    <asp:Button ID="btnLimpiar" runat="server" Text=" Limpiar" CssClass="btn" OnClick="btnLimpiar_Click" />
                </div>
                
            </main>
        </div>
    </form>
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