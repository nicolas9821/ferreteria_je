<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inicio.aspx.cs" Inherits="ferreteria_je.inicio" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Inicio - Administrador Ferretería</title>
    <link rel="stylesheet" href="../estilos/inicio.css" />
    <link rel="stylesheet" href="../estilos/panel_inicio.css" />
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
    <style>
        /* Estilos adicionales para las tablas con scroll */
        .table-scroll-container {
            max-height: 400px; /* Altura máxima antes de que aparezca el scroll */
            overflow-y: auto; /* Habilita el scroll vertical */
            border: 1px solid #e0e0e0;
            margin-bottom: 20px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            padding: 15px; /* Añadido padding para el contenido interno */
        }

        .table-scroll-container h3 {
            padding-bottom: 10px; /* Espacio debajo del título de la tabla */
            margin-top: 0;
            margin-bottom: 15px; /* Espacio para el filtro */
            background-color: #fff; /* Asegura que el título no tenga fondo de color */
            border-bottom: 1px solid #e0e0e0;
            color: #333;
            font-size: 1.1em;
            position: sticky;
            top: 0;
            z-index: 2; /* Mayor z-index para el título sticky */
        }

        .data-gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px; /* Espacio para el filtro */
        }

        .data-gridview th, .data-gridview td {
            padding: 12px 15px;
            border-bottom: 1px solid #e0e0e0;
            text-align: left;
            font-size: 0.9em;
        }

        .data-gridview th {
            background-color: #f2f2f2;
            color: #555;
            font-weight: bold;
            position: sticky; /* Fija los encabezados al hacer scroll */
            top: 50px; /* Ajusta este valor si el título también es sticky */
            z-index: 1; /* Asegura que los encabezados estén sobre las filas al hacer scroll */
        }

        .data-gridview tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .data-gridview tr:hover {
            background-color: #f1f1f1;
        }

        .full-width-card {
            grid-column: 1 / -1; /* Hace que esta tarjeta ocupe todo el ancho de la cuadrícula */
        }

        .export-button {
            background-color: #28a745; /* Color verde */
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 1em;
            transition: background-color 0.3s ease;
            margin-top: 20px; /* Espacio encima del botón */
        }

        .export-button:hover {
            background-color: #218838;
        }

        .filter-controls {
            display: flex;
            gap: 10px;
            margin-bottom: 10px;
            align-items: center;
        }
        .filter-controls input[type="text"], .filter-controls input[type="date"] {
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            flex-grow: 1; /* Permite que el input crezca */
            max-width: 300px; /* Ancho máximo para inputs */
        }
        .filter-controls .btn-filter, .filter-controls .btn-clear-filter {
            padding: 8px 15px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.9em;
            transition: background-color 0.3s ease;
        }
        .filter-controls .btn-filter {
            background-color: #007bff;
            color: white;
        }
        .filter-controls .btn-filter:hover {
            background-color: #0056b3;
        }
        .filter-controls .btn-clear-filter {
            background-color: #dc3545;
            color: white;
        }
        .filter-controls .btn-clear-filter:hover {
            background-color: #c82333;
        }
    </style>
</head>
<body>
    <div class="dashboard">
        <aside class="sidebar">
            <div style="text-align: center;">
                <img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria" />
            </div>
            <nav>
                <ul>
                    <li><a href="inicio.aspx" class="activo">Inicio</a></li>
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
            <form id="form1" runat="server">
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                <div class="topbar">
                    <div class="user-menu">
                        <asp:LinkButton ID="lnkCerrarSesion" runat="server" CssClass="logout-btn" OnClick="lnkCerrarSesion_Click">
                            <i class="fas fa-sign-out-alt"></i> Cerrar sesión
                        </asp:LinkButton>
                        <div class="user-name" onclick="toggleDropdown()">
                            <i class="fas fa-user-circle"></i> <asp:Literal ID="litUserNameButton" runat="server"></asp:Literal>
                        </div>
                        <div class="dropdown" id="userDropdown">
                            <div><strong>Nombre:</strong> <asp:Literal ID="litUserFullName" runat="server"></asp:Literal></div>
                            <div><strong>Correo:</strong> <asp:Literal ID="litUserEmail" runat="server"></asp:Literal></div>
                            <div><strong>Teléfono:</strong> <asp:Literal ID="litUserPhone" runat="server"></asp:Literal></div>
                            <div><a href="NUEVACONTRASEÑA.aspx">Cambiar contraseña</a></div>
                        </div>
                    </div>
                </div>

                <h1>Panel de Administración</h1>

                <asp:UpdatePanel ID="UpdatePanelKPIs" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="kpi-container">
                            <a href="ventas.aspx" class="kpi-card ventas-hoy" data-icon="&#xf65d;">
                                <h3>Ventas (Hoy)</h3>
                                <p><asp:Label ID="lblVentasHoy" runat="server"></asp:Label></p>
                            </a>
                            <a href="productos.aspx" class="kpi-card productos-stock" data-icon="&#xf06a;">
                                <h3>Productos Bajo Stock</h3>
                                <p><asp:Label ID="lblProductosBajoStock" runat="server"></asp:Label></p>
                            </a>
                            <a href="ventas.aspx" class="kpi-card ventas-mes" data-icon="&#xf201;">
                                <h3>Ventas (Mes)</h3>
                                <p><asp:Label ID="lblCantidadVentas" runat="server"></asp:Label></p>
                            </a>
                            <a href="clientes.aspx" class="kpi-card clientes" data-icon="&#xf007;">
                                <h3>Clientes</h3>
                                <p><asp:Label ID="lblCantidadClientes" runat="server"></asp:Label></p>
                            </a>
                            <a href="compra.aspx" class="kpi-card compras" data-icon="&#xf543;">
                                <h3>Compras</h3>
                                <p><asp:Label ID="lblCantidadCompras" runat="server"></asp:Label></p>
                            </a>
                            <a href="productos.aspx" class="kpi-card total-productos" data-icon="&#xf466;">
                                <h3>Total Productos</h3>
                                <p><asp:Label ID="lblCantidadProductos" runat="server"></asp:Label></p>
                            </a>
                            <a href="proveedores.aspx" class="kpi-card proveedores" data-icon="&#xf0d1;">
                                <h3>Proveedores</h3>
                                <p><asp:Label ID="lblCantidadProveedores" runat="server"></asp:Label></p>
                            </a>
                            <a href="usuarios.aspx" class="kpi-card usuarios" data-icon="&#xf0c0;">
                                <h3>Usuarios</h3>
                                <p><asp:Label ID="lblCantidadUsuarios" runat="server"></asp:Label></p>
                            </a>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="dashboard-grid">
                    <%-- Sección de Últimos Pedidos Registrados --%>
                    <asp:UpdatePanel ID="UpdatePanelLatestOrders" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="dashboard-card">
                                <h2>Últimos Pedidos Registrados</h2>
                                <ul class="latest-orders-list">
                                    <asp:Repeater ID="rptLatestOrders" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <i class="fas fa-receipt"></i>
                                                <div>
                                                    <strong>Pedido #<%# Eval("id_venta") %></strong> -
                                                    Cliente: <%# Eval("nombre_cliente") %><br />
                                                    Total: <%# Eval("total_venta", "{0:C}") %> -
                                                    Fecha: <%# Eval("fecha", "{0:dd/MM/yyyy}") %>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Literal ID="litNoLatestOrders" runat="server" Text="<li><i class='fas fa-info-circle'></i> No hay pedidos recientes.</li>" Visible="false"></asp:Literal>
                                </ul>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <%-- Sección de Alertas de Stock Bajo (Ahora con GridView) --%>
                    <asp:UpdatePanel ID="UpdatePanelLowStock" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="dashboard-card">
                                <h2>Alertas de Stock Bajo</h2>
                                <asp:GridView ID="gvLowStockProducts" runat="server" AutoGenerateColumns="False"
                                    CssClass="low-stock-gridview" EmptyDataText="No hay productos con stock bajo.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Producto">
                                            <ItemTemplate>
                                                <i class="fas fa-exclamation-triangle low-stock-item"></i> <%# Eval("nombre") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="stock" HeaderText="Stock Actual" ItemStyle-CssClass="low-stock-item" />
                                    </Columns>
                                    <HeaderStyle CssClass="low-stock-gridview-header" />
                                    <RowStyle CssClass="low-stock-gridview-row" />
                                    <AlternatingRowStyle CssClass="low-stock-gridview-altrow" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <%-- Otras tarjetas o contenido existente aquí (si lo hubiera) --%>

                </div> <%-- Fin de dashboard-grid --%>

                <%-- *** INICIO DE LA SECCIÓN DE MULTITABLA (INFORMACIÓN DETALLADA) AL FINAL *** --%>
                <asp:UpdatePanel ID="UpdatePanelAllTables" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="dashboard-card full-width-card">
                            <h2>Información Detallada</h2> <%-- Título cambiado --%>

                            <%-- Tabla de Productos --%>
                            <div class="table-scroll-container">
                                <h3>Productos</h3>
                                <div class="filter-controls">
                                    <asp:TextBox ID="txtFilterProductos" runat="server" Placeholder="Filtrar por nombre"></asp:TextBox>
                                    <asp:Button ID="btnFilterProductos" runat="server" Text="Filtrar" OnClick="btnFilterProductos_Click" CssClass="btn-filter" />
                                    <asp:Button ID="btnClearFilterProductos" runat="server" Text="Quitar Filtro" OnClick="btnClearFilterProductos_Click" CssClass="btn-clear-filter" />
                                </div>
                                <asp:GridView ID="gvTodosProductos" runat="server" AutoGenerateColumns="False"
                                    CssClass="data-gridview" EmptyDataText="No hay productos para mostrar.">
                                    <Columns>
                                        <asp:BoundField DataField="id_producto" HeaderText="ID" />
                                        <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                                        <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                        <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="stock" HeaderText="Stock" />
                                    </Columns>
                                    <HeaderStyle CssClass="data-gridview-header" />
                                    <RowStyle CssClass="data-gridview-row" />
                                    <AlternatingRowStyle CssClass="data-gridview-altrow" />
                                </asp:GridView>
                            </div>

                            <%-- Tabla de Ventas --%>
                            <div class="table-scroll-container">
                                <h3>Ventas</h3>
                                <div class="filter-controls">
                                    <asp:TextBox ID="txtFilterVentas" runat="server" TextMode="Date"></asp:TextBox> <%-- Filtro por fecha --%>
                                    <asp:Button ID="btnFilterVentas" runat="server" Text="Filtrar" OnClick="btnFilterVentas_Click" CssClass="btn-filter" />
                                    <asp:Button ID="btnClearFilterVentas" runat="server" Text="Quitar Filtro" OnClick="btnClearFilterVentas_Click" CssClass="btn-clear-filter" />
                                </div>
                                <asp:GridView ID="gvTodasVentas" runat="server" AutoGenerateColumns="False"
                                    CssClass="data-gridview" EmptyDataText="No hay ventas para mostrar.">
                                    <Columns>
                                        <asp:BoundField DataField="id_venta" HeaderText="ID Venta" />
                                        <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="precio_unitario" HeaderText="Precio Unit." DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" />
                                        <asp:BoundField DataField="total" HeaderText="Total Venta" DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="id_cliente" HeaderText="ID Cliente" />
                                        <asp:BoundField DataField="id_usuario" HeaderText="ID Usuario" />
                                        <asp:BoundField DataField="id_producto" HeaderText="ID Producto" />
                                    </Columns>
                                    <HeaderStyle CssClass="data-gridview-header" />
                                    <RowStyle CssClass="data-gridview-row" />
                                    <AlternatingRowStyle CssClass="data-gridview-altrow" />
                                </asp:GridView>
                            </div>

                            <%-- Tabla de Clientes --%>
                            <div class="table-scroll-container">
                                <h3>Clientes</h3>
                                <div class="filter-controls">
                                    <asp:TextBox ID="txtFilterClientes" runat="server" Placeholder="Filtrar por nombre"></asp:TextBox>
                                    <asp:Button ID="btnFilterClientes" runat="server" Text="Filtrar" OnClick="btnFilterClientes_Click" CssClass="btn-filter" />
                                    <asp:Button ID="btnClearFilterClientes" runat="server" Text="Quitar Filtro" OnClick="btnClearFilterClientes_Click" CssClass="btn-clear-filter" />
                                </div>
                                <asp:GridView ID="gvTodosClientes" runat="server" AutoGenerateColumns="False"
                                    CssClass="data-gridview" EmptyDataText="No hay clientes para mostrar.">
                                    <Columns>
                                        <asp:BoundField DataField="id_cliente" HeaderText="ID Cliente" />
                                        <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                                        <asp:BoundField DataField="telefono" HeaderText="Teléfono" />
                                        <asp:BoundField DataField="direccion" HeaderText="Dirección" />
                                        <asp:BoundField DataField="email" HeaderText="Email" />
                                        <asp:BoundField DataField="cedula" HeaderText="Cédula" />
                                    </Columns>
                                    <HeaderStyle CssClass="data-gridview-header" />
                                    <RowStyle CssClass="data-gridview-row" />
                                    <AlternatingRowStyle CssClass="data-gridview-altrow" />
                                </asp:GridView>
                            </div>

                            <%-- Tabla de Compras --%>
                            <div class="table-scroll-container">
                                <h3>Compras</h3>
                                <div class="filter-controls">
                                    <asp:TextBox ID="txtFilterCompras" runat="server" TextMode="Date"></asp:TextBox> <%-- Filtro por fecha --%>
                                    <asp:Button ID="btnFilterCompras" runat="server" Text="Filtrar" OnClick="btnFilterCompras_Click" CssClass="btn-filter" />
                                    <asp:Button ID="btnClearFilterCompras" runat="server" Text="Quitar Filtro" OnClick="btnClearFilterCompras_Click" CssClass="btn-clear-filter" />
                                </div>
                                <asp:GridView ID="gvTodasCompras" runat="server" AutoGenerateColumns="False"
                                    CssClass="data-gridview" EmptyDataText="No hay compras para mostrar.">
                                    <Columns>
                                        <asp:BoundField DataField="id_compra" HeaderText="ID Compra" />
                                        <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="total" HeaderText="Total Compra" DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="id_proveedor" HeaderText="ID Proveedor" />
                                        <asp:BoundField DataField="nombre_proveedor" HeaderText="Proveedor" />
                                        <asp:BoundField DataField="id_usuario" HeaderText="ID Usuario" />
                                        <asp:BoundField DataField="nombre_usuario" HeaderText="Usuario" />
                                    </Columns>
                                    <HeaderStyle CssClass="data-gridview-header" />
                                    <RowStyle CssClass="data-gridview-row" />
                                    <AlternatingRowStyle CssClass="data-gridview-altrow" />
                                </asp:GridView>
                            </div>

                            <%-- Tabla de Proveedores --%>
                            <div class="table-scroll-container">
                                <h3>Proveedores</h3>
                                <div class="filter-controls">
                                    <asp:TextBox ID="txtFilterProveedores" runat="server" Placeholder="Filtrar por nombre"></asp:TextBox>
                                    <asp:Button ID="btnFilterProveedores" runat="server" Text="Filtrar" OnClick="btnFilterProveedores_Click" CssClass="btn-filter" />
                                    <asp:Button ID="btnClearFilterProveedores" runat="server" Text="Quitar Filtro" OnClick="btnClearProveedores_Click" CssClass="btn-clear-filter" />
                                </div>
                                <asp:GridView ID="gvTodosProveedores" runat="server" AutoGenerateColumns="False"
                                    CssClass="data-gridview" EmptyDataText="No hay proveedores para mostrar.">
                                    <Columns>
                                        <asp:BoundField DataField="id_proveedor" HeaderText="ID Proveedor" />
                                        <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                                        <asp:BoundField DataField="telefono" HeaderText="Teléfono" />
                                        <asp:BoundField DataField="direccion" HeaderText="Dirección" />
                                        <asp:BoundField DataField="email" HeaderText="Email" />
                                    </Columns>
                                    <HeaderStyle CssClass="data-gridview-header" />
                                    <RowStyle CssClass="data-gridview-row" />
                                    <AlternatingRowStyle CssClass="data-gridview-altrow" />
                                </asp:GridView>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <%-- *** Botón para exportar a Excel (AHORA FUERA DEL UpdatePanel) *** --%>
                <div style="margin-top: 20px; text-align: right;">
                    <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel (Tablas Visibles)" OnClick="btnExportarExcel_Click" CssClass="export-button" />
                </div>
                
            </form>
        </main>
    </div>
    <script>
        // Funciones JavaScript existentes...
        function toggleDropdown() {
            const dropdown = document.getElementById('userDropdown');
            dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        }

        document.addEventListener('click', function (e) {
            const menu = document.querySelector('.user-menu');
            if (menu && !menu.contains(e.target)) {
                const dropdown = document.getElementById('userDropdown');
                if (dropdown) {
                    dropdown.style.display = 'none';
                }
            }
        });

        document.addEventListener('DOMContentLoaded', function () {
            const currentPath = window.location.pathname.split('/').pop();
            const sidebarLinks = document.querySelectorAll('.sidebar nav ul li a');

            sidebarLinks.forEach(link => {
                if (link.getAttribute('href') === currentPath) {
                    link.classList.add('activo');
                } else {
                    link.classList.remove('activo');
                }
            });
        });
    </script>
</body>
</html>