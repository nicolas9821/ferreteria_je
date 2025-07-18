<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="auxiliar_gestion_proveedor.aspx.cs" Inherits="ferreteria_je.auxiliar_gestion_proveedor" %>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
  <title>Proveedores</title>
  <link rel="stylesheet" href="../estilos/styless.css" />
  <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</head>
<body>
<div class="dashboard">
  <aside class="sidebar">
    <div style="text-align: center;"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería" class="logo-ferreteria"/></div>
    <nav>
      <ul>
        <li><a href="auxiliar_proveedor.aspx">Proveedores</a></li>
        <li><a href="auxiliar_producto.aspx">Productos</a></li>
        <li><a href="auxiliar_compra.aspx">Compras</a></li>
      </ul>
    </nav>
  </aside>

  <main class="main-content">
    <h1>Gestión de Productos</h1>
    <form id="formProveedor">
      <input type="text" placeholder="NIT o ID proveedor" required />
      <input type="text" placeholder="Nombre del proveedor" required />
      <input type="text" placeholder="Teléfono" />
      <input type="email" placeholder="Correo" />
      <input type="text" placeholder="Dirección" />
      <div class="acciones">
        <button type="button"><i class="fas fa-plus"></i> Agregar</button>
        <button type="button"><i class="fas fa-pen"></i> Actualizar</button>
      </div>
    </form>
  </main>
</div>
</body>
</html>
