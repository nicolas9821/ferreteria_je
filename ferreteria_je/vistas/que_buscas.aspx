<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="que_buscas.aspx.cs" Inherits="ferreteria_je.que_buscas" %>

<!DOCTYPE html>
<html lang="es">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>¿Qué Buscas? - Ferretería JE</title>
    <link
      href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap"
      rel="stylesheet"
    />

    <link
      href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
      rel="stylesheet"
      integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"
      crossorigin="anonymous"
    />
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link
      href="https://fonts.googleapis.com/css2?family=Pacifico&family=Quicksand:wght@300..700&display=swap"
      rel="stylesheet"
    />
    <link
      rel="stylesheet"
      href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css"
    />
    <link rel="stylesheet" href="../estilos/que_buscas.css" />
    </head>
  <body>
    <nav class="navbar" role="navigation" aria-label="Menú principal">
      <div class="logo">
        <a href="dashboard.ASPX"
          ><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería JE"
        /></a>
      </div>
      <div class="nav-links">
        <a href="dashboard.ASPX">Home</a>
        <a href="que_buscas.ASPX">¿Qué buscas?</a>
        <a href="mision_vision.ASPX">Misión y Visión</a>
        <a href="quienes_somos.ASPX">¿Quiénes somos?</a>
      </div>
      <div class="auth-links">
        <a href="login.ASPX">Iniciar Sesión</a>
        <a href="registro.ASPX">Crear Cuenta</a>
      </div>
    </nav>

    <div class="author-date">Junio 4, 2025 por <a href="#">Gaes 8</a></div>
    <section class="hero-section">
      <div class="hero-overlay">
        <h1>¡Bienvenidos a JE Ferretería!</h1>
        <p class="subtitle">Todo en herramientas, tornillos, pintura, plomería y más</p>
      </div>
    </section>
    <main class="main-content">
      <div class="search-filter-container">
        <div class="search-bar">
          <input type="text" placeholder="Buscar productos..." />
          <i class="bi bi-search search-icon"></i>
        </div>
        <div class="filter-sort-options">
          <select name="filter" id="filter">
            <option value="">Filtrar por categoría</option>
            <option value="electricidad">Electricidad</option>
            <option value="plomeria">Plomería</option>
            <option value="construccion">Construcción</option>
            <option value="herramientas">Herramientas</option>
            <option value="pintura">Pintura</option>
          </select>
          <select name="sort" id="sort">
            <option value="">Ordenar por</option>
            <option value="az">Nombre (A-Z)</option>
            <option value="za">Nombre (Z-A)</option>
            <option value="price-asc">Precio (Menor a Mayor)</option>
            <option value="price-desc">Precio (Mayor a Menor)</option>
          </select>
        </div>
      </div>

      <section class="category-section">
        <h2>Nuestras Categorías Principales</h2>
        <div class="category-grid">
          <a href="clientes.aspx" class="category-item">
            <img src="electricidad.jpg" alt="Electricidad" />
            <h3>Electricidad</h3>
            <p>Encuentra todo lo que necesitas para instalaciones eléctricas, iluminación y más.</p>
          </a>
          <a href="clientes.aspx" class="category-item">
            <img src="plomería.jpg" alt="Plomería" />
            <h3>Plomería</h3>
            <p>Soluciones completas para sistemas de agua, tuberías y reparaciones.</p>
          </a>
          <a href="clientes.aspx" class="category-item">
            <img src="construcción.jpg" alt="Construcción" />
            <h3>Construcción</h3>
            <p>Materiales y herramientas para tus proyectos de obra, grandes y pequeños.</p>
          </a>
        </div>
      </section>
    </main>

    <footer class="main-footer">
      <p>© 2025 Ferrelectricos JE - Todos los derechos reservados. ¡Bienvenidos a la ferretería!</p>
      <div class="social-links-footer">
        <a
          href="https://wa.me/573102522316"
          target="_blank"
          rel="noopener noreferrer"
          aria-label="WhatsApp de Ferrelectricos"
          class="social-icon-whatsapp"
        >
          <i class="bi bi-whatsapp icon-large"></i>
        </a>

        <a
          href="https://www.instagram.com/je_ferrelectricos/"
          target="_blank"
          rel="noopener noreferrer"
          aria-label="Instagram de Ferrelectricos"
          class="social-icon-instagram"
        >
          <i class="bi bi-instagram icon-large"></i>
        </a>

        <a
          href="mailto:Ferrelectricos@gmail.com"
          target="_blank"
          rel="noopener noreferrer"
          aria-label="Correo de Ferrelectricos"
          class="social-icon-gmail"
        >
          <i class="bi bi-envelope-fill icon-large"></i>
        </a>
      </div>
    </footer>
  </body>
</html>
