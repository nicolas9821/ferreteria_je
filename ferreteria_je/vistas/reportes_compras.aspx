<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reportes_compras.aspx.cs" Inherits="ferreteria_je.vistas.reportes_compras" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reporte de Compras</title>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="../estilos/inicio.css" /> <%-- Asegúrate de que esta ruta sea correcta --%>
    <link rel="stylesheet" href="../estilos/proveedores.css" />
    <style>
        html, body {
            height: 100%; /* Ensure HTML and Body take full viewport height */
            margin: 0;
            padding: 0;
            overflow: hidden; /* Prevent overall page scroll, let dashboard handle it */
        }

        .dashboard {
            display: flex; /* Makes dashboard a flex container */
            height: 100vh; /* Make dashboard fill the entire viewport height */
            width: 100%;
        }

        .main-content {
            flex-grow: 1; /* Allows main-content to take up all remaining horizontal space */
            overflow-y: auto; /* This is the critical part for vertical scrolling */
            padding: 20px; /* Add some padding for visual comfort */
            box-sizing: border-box; /* Include padding in the element's total width and height */
        }

        /* Your existing styles (ensure these don't conflict with the above flex properties) */
        .form-inline .form-group { margin-right: 15px; }
        .gridview-container { margin-top: 20px; overflow-x: auto; margin-bottom: 30px; }
        .total-summary { margin-top: 20px; font-size: 1.2em; font-weight: bold; }
        h2 { margin-top: 40px; margin-bottom: 20px; color: #333; }
    </style>
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
                        <li><a href="reportes.aspx" class="activo">Reportes</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <h1>Reporte General de Compras</h1>

                <div class="form-inline mb-4">
                    <div class="form-group">
                        <label for="txtFechaInicio">Fecha Inicio:</label>
                        <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtFechaFin">Fecha Fin:</label>
                        <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnGenerarReporte" runat="server" Text="Generar Reporte" OnClick="btnGenerarReporte_Click" CssClass="btn btn-primary" />
                </div>

                <h2>Detalle de Compras por Ítem</h2>
                <div class="gridview-container">
                    <asp:GridView ID="gvCompras" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
                        EmptyDataText="No hay datos de compras para mostrar." DataKeyNames="IdCompra">
                        <Columns>
                            <asp:BoundField DataField="IdCompra" HeaderText="ID Compra" />
                            <asp:BoundField DataField="FechaCompra" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="NombreProveedor" HeaderText="Proveedor" />
                            <asp:BoundField DataField="NombreUsuario" HeaderText="Comprador" />
                            <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                            <asp:BoundField DataField="CantidadComprada" HeaderText="Cant." />
                            <asp:BoundField DataField="PrecioUnitarioCompra" HeaderText="Precio Unit." DataFormatString="{0:C}" />
                            <asp:BoundField DataField="SubtotalLinea" HeaderText="Subtotal" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="TotalCompra" HeaderText="Total Compra" DataFormatString="{0:C}" />
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="total-summary">
                    <asp:Label ID="lblTotalGeneralCompras" runat="server" Text="Total General de Compras: $0.00"></asp:Label>
                </div>

                <h2>Compras Consolidadas por Proveedor</h2>
                <div class="gridview-container">
                    <asp:GridView ID="gvComprasPorProveedor" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
                        EmptyDataText="No hay datos de compras por proveedor para mostrar.">
                        <Columns>
                            <asp:BoundField DataField="IdProveedorAgregado" HeaderText="ID Proveedor" />
                            <asp:BoundField DataField="NombreProveedorAgregado" HeaderText="Proveedor" />
                            <asp:BoundField DataField="TotalComprasAgregado" HeaderText="Total Comprado" DataFormatString="{0:C}" />
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="total-summary">
                    <asp:Label ID="lblTotalGeneralComprasProveedor" runat="server" Text="Total General Comprado a Proveedores: $0.00"></asp:Label>
                </div>

                <p><a href="reportes.aspx">Volver a Selección de Reportes</a></p>
            </main>
        </div>
    </form>
</body>
</html>