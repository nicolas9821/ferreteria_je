<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mision_vision.aspx.cs" Inherits="ferreteria_je.mision_vision" %>

<!DOCTYPE html>
<html lang="es">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Misión y Visión - Ferretería JE</title>
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
    <link rel="stylesheet" href="../estilos/mision_vision.css" />
  </head>
  <body>
    <form id="form1" runat="server"> <%-- Agrega el form runat="server" para que los controles de servidor funcionen --%>
        <nav class="navbar" role="navigation" aria-label="Menú principal">
          <div class="logo">
            <a href="dashboard.aspx"
              ><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería JE"
            /></a>
          </div>
          <div class="nav-links">
            <a href="dashboard.aspx">Home</a>
            <%-- <a href="que_buscas.aspx">¿Qué buscas?</a> --%> <%-- Elimina esta línea --%>
            <a href="mision_vision.aspx">Misión y Visión</a>
            <a href="quienes_somos.aspx">¿Quiénes somos?</a>
          </div>
          <div class="auth-links">
            <a href="login.aspx">Iniciar Sesión</a>
            <a href="registro.aspx">Crear Cuenta</a>
          </div>
        </nav>

        <div class="author-date">
            <span id="currentDate" runat="server"></span> por <a href="#">Gaes 8</a>
        </div>
        <section class="hero-section">
          <div class="hero-overlay">
            <h1>¡Bienvenidos a JE Ferretería!</h1>
            <p class="subtitle">Todo en herramientas, tornillos, pintura, plomería y más</p>
          </div>
        </section>
        <main class="main-content">
          <section id="mision-vision" class="mision-vision">
            <h2>¿QUIÉNES SOMOS?</h2>
            <div>
              <div>
                <h3><i class="bi bi-bullseye"></i> Misión</h3>
                <p>
                  Impulsar los sueños de quienes construyen, reparan y transforman el mundo que los
                  rodea. En JE Ferretería no solo vendemos herramientas,
                  <strong>entregamos soluciones</strong> que generan progreso, confianza y seguridad.
                  Nuestro propósito es ser el aliado de cada idea, desde un pequeño arreglo hasta una
                  gran obra.
                </p>
              </div>
              <div>
                <h3><i class="bi bi-binoculars-fill"></i> Visión</h3>
                <p>
                  Revolucionar la experiencia ferretera: ser más que una tienda,
                  <strong>ser una comunidad</strong> que conecta personas, proyectos y propósitos. En
                  2030, JE Ferretería será referente nacional por su innovación digital, asesoría
                  personalizada, responsabilidad social y presencia sólida en cada rincón del país.
                </p>
              </div>
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
    </form> <%-- Cierra el form --%>
  </body>
</html>