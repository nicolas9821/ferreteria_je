<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestionventas.aspx.cs" Inherits="ferreteria_je.gestionventas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Ventas</title>
    <link rel="stylesheet" href="../estilos/styless.css" />
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>

    <style>
        /* Aseguramos que el HTML y BODY puedan desplazarse si el contenido es largo */
        html, body {
            height: 100%; /* Asegura que html y body ocupen al menos el 100% de la altura del viewport */
            margin: 0;
            padding: 0;
            overflow-y: auto; /* Habilita el desplazamiento vertical en toda la ventana si el contenido excede la altura */
        }

        /* Si el dashboard es un contenedor flex (sidebar y main-content), esto es clave */
        .dashboard {
            display: flex; /* Asegura que el dashboard sea un contenedor flex */
            min-height: 100vh; /* Permite que el dashboard crezca más allá del 100% del viewport si es necesario */
        }

        /* Estilos para el main-content (área principal) */
        .main-content {
            flex-grow: 1; /* Permite que ocupe todo el espacio restante en un contenedor flex (dashboard) */
            padding: 20px; /* Un poco de padding general */
            padding-bottom: 80px; /* Aumenta el padding inferior para dar espacio a los botones */
            box-sizing: border-box; /* Incluye padding en el tamaño total */
            overflow-y: visible; /* Importante: permite que el scroll se gestione por el body/html */
        }
        
        /* Contenedor del formulario */
        .form-container {
            width: 100%;
            max-width: 600px; /* Limita el ancho del formulario para que no sea demasiado grande */
            margin: 20px auto; /* Centra el formulario y añade margen superior/inferior */
            padding: 25px; /* Espacio interno generoso */
            background-color: #ffffff; /* Fondo blanco */
            border-radius: 10px; /* Bordes más redondeados */
            box-shadow: 0 4px 12px rgba(0,0,0,0.1); /* Sombra más pronunciada */
            box-sizing: border-box;
            display: block; /* Asegura que se comporte como un bloque */
        }

        /* Estilos para los labels */
        .form-container label {
            display: block; /* Cada label en su propia línea */
            margin-bottom: 8px; /* Espacio debajo del label */
            margin-top: 15px; /* Espacio encima del label para separarlo del campo anterior */
            font-weight: bold; /* Texto en negrita para los labels */
            color: #333; /* Color de texto oscuro */
            font-size: 1.05em; /* Un poco más grande que lo normal */
        }
        /* Ajuste para el primer label para que no tenga margen superior */
        .form-container label:first-of-type {
            margin-top: 0;
        }

        /* Estilos para los campos de entrada (TextBox y DropDownList) */
        .input-field {
            width: calc(100% - 24px); /* Ancho completo menos padding y borde */
            padding: 12px; /* Padding interno */
            border: 1px solid #dcdcdc; /* Borde gris claro */
            border-radius: 6px; /* Bordes redondeados */
            margin-bottom: 20px; /* Espacio debajo de cada campo */
            box-sizing: border-box;
            font-size: 1em; /* Tamaño de fuente legible */
            color: #555; /* Color de texto */
        }

        .input-field:focus {
            border-color: #007bff; /* Borde azul al enfocar */
            outline: none; /* Eliminar el contorno por defecto */
            box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.25); /* Sombra suave al enfocar */
        }

        /* Estilos específicos para DropDownList */
        .input-field[id^="ddl"] { /* Selecciona IDs que comienzan con "ddl" */
            appearance: none; /* Elimina la flecha predeterminada del navegador */
            background-image: url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%22292.4%22%20height%3D%22292.4%22%3E%3Cpath%20fill%3D%22%23007bff%22%20d%3D%22M287%2069.4a17.6%2017.6%200%200%200-13.2-6.4H18.4c-4.8%200-9.2%202.4-13.2%206.4-8%208-8%2020.8%200%2028.8l128%20128c4%204%209.2%206.4%2014.4%206.4s10.4-2.4%2014.4-6.4l128-128c8-8%208-20.8%200-28.8z%22%2F%3E%3C%2Fsvg%3E'); /* Flecha SVG azul */
            background-repeat: no-repeat;
            background-position: right 12px center; /* Posiciona la flecha */
            background-size: 14px; /* Tamaño de la flecha */
            padding-right: 40px; /* Espacio extra a la derecha para la flecha */
            cursor: pointer;
        }

        /* Contenedor de los botones */
        .acciones {
            display: flex !important; /* Forzar flexbox para alinear botones */
            justify-content: center; /* Centrar los botones horizontalmente */
            flex-wrap: wrap; /* Permite que los botones salten de línea en pantallas pequeñas */
            gap: 20px; /* Espacio entre los botones */
            margin-top: 40px; /* Margen superior más grande para separar del formulario */
            width: 100%; /* Asegura que ocupe todo el ancho disponible */
        }

        /* Estilos para los botones */
        .btn {
            display: inline-block !important; /* Asegurar que los botones se muestren como bloques en línea */
            padding: 12px 25px; /* Padding generoso */
            min-width: 120px; /* Ancho mínimo para los botones */
            background-color: #007bff; /* Color azul primario */
            color: white; /* Texto blanco */
            border: none; /* Sin borde */
            border-radius: 5px; /* Bordes ligeramente redondeados */
            cursor: pointer; /* Cursor de puntero */
            font-size: 1em; /* Tamaño de fuente */
            font-weight: bold; /* Texto en negrita */
            text-align: center;
            text-decoration: none;
            transition: background-color 0.3s ease, transform 0.2s ease; /* Transiciones suaves */
            box-shadow: 0 2px 4px rgba(0,0,0,0.15); /* Sombra ligera */
        }

        .btn:hover {
            background-color: #0056b3; /* Color azul más oscuro al pasar el ratón */
            transform: translateY(-2px); /* Pequeño efecto de elevación al pasar el ratón */
        }

        .btn:active {
            transform: translateY(0); /* Vuelve a su posición normal al hacer clic */
            box-shadow: 0 1px 2px rgba(0,0,0,0.2); /* Sombra más pequeña al hacer clic */
        }

        /* Estilo para el mensaje al usuario */
        .mensaje {
            text-align: center;
            margin-top: 25px; /* Espacio superior */
            margin-bottom: 25px; /* Espacio inferior */
            font-weight: bold;
            display: block; 
            font-size: 1.1em;
            color: #28a745; /* Ejemplo de color verde para éxito */
            /* Puedes usar un color diferente para mensajes de error, por ejemplo: .mensaje.error { color: #dc3545; } */
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="dashboard">
            <aside class="sidebar">
                <div style="text-align: center;">
                    <img src="../imagenes/Logo JE.jpg" alt="Logo Ferreteria" class="logo-ferreteria" />
                </div>
                <nav>
                    <ul>
                        <li><a href="inicio.aspx">Inicio</a></li>
                        <li><a href="proveedores.aspx">Proveedores</a></li>
                        <li><a href="productos.aspx">Productos</a></li>
                        <li><a href="ventas.aspx" class="activo">Ventas</a></li>
                        <li><a href="usuarios.aspx">Usuarios</a></li>
                        <li><a href="clientes.aspx">Clientes</a></li>
                        <li><a href="facturas.aspx">Facturas</a></li>
                        <li><a href="compra.aspx">Compras</a></li>
                        <li><a href="reportes.aspx">Reportes</a></li>
                    </ul>
                </nav>
            </aside>

            <main class="main-content">
                <h1>Gestión de Ventas</h1>
                
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

                <div class="form-container">
                    <asp:Label ID="lblIdVenta" runat="server" Text="ID Venta:"></asp:Label>
                    <asp:TextBox ID="txtIdVenta" runat="server" CssClass="input-field" placeholder="ID Venta (para actualizar/eliminar)" ReadOnly="True"></asp:TextBox>
                    
                    <asp:Label ID="lblFecha" runat="server" Text="Fecha:"></asp:Label>
                    <asp:TextBox ID="txtFecha" runat="server" CssClass="input-field" TextMode="Date" placeholder="Fecha de Venta" required="required"></asp:TextBox>
                    
                    <asp:Label ID="lblCliente" runat="server" Text="Cliente:"></asp:Label>
                    <asp:DropDownList ID="ddlCliente" runat="server" CssClass="input-field"></asp:DropDownList>

                    <asp:Label ID="lblUsuario" runat="server" Text="Usuario:"></asp:Label>
                    <asp:DropDownList ID="ddlUsuario" runat="server" CssClass="input-field"></asp:DropDownList>
                    
                    <asp:Label ID="lblProducto" runat="server" Text="Producto:"></asp:Label>
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="input-field" AutoPostBack="True" OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged"></asp:DropDownList>
                    
                    <asp:Label ID="lblCantidad" runat="server" Text="Cantidad:"></asp:Label>
                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="input-field" placeholder="Cantidad" TextMode="Number" AutoPostBack="True" OnTextChanged="txtCantidad_TextChanged"></asp:TextBox>
                    
                    <asp:Label ID="lblPrecioUnitario" runat="server" Text="Precio Unitario:"></asp:Label>
                    <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="input-field" placeholder="Precio Unitario"></asp:TextBox>
                    
                    <asp:Label ID="lblTotalVenta" runat="server" Text="Total Venta:"></asp:Label>
                    <asp:TextBox ID="txtTotalVenta" runat="server" CssClass="input-field" placeholder="Total de Venta" ReadOnly="True"></asp:TextBox>
                </div>

                <div class="acciones">
                    <asp:Button ID="btnAgregar" runat="server" Text=" Agregar" CssClass="btn" OnClick="btnAgregar_Click" />
                    <asp:Button ID="btnActualizar" runat="server" Text=" Actualizar" CssClass="btn" OnClick="btnActualizar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text=" Eliminar" CssClass="btn" OnClick="btnEliminar_Click" OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta venta?');" />
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