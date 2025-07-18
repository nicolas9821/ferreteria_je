<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="quienes_somos.aspx.cs" Inherits="ferreteria_je.quienes_somos1" %>

<!DOCTYPE html>
<html lang="es">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>¿Quiénes somos? - Ferretería JE</title>
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
    <link rel="stylesheet" href="../estilos/quienes_somos.css" />
  </head>
  <body>
    <form id="form1" runat="server"> <%-- Asegúrate de tener el form con runat="server" --%>
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
          <section id="mapa">
            <h2>Encuéntranos</h2>
            <p>Dirección: Cra. 3 #136 Sur-42, Bogotá</p>
            <div>
              <iframe
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3976.012351478522!2d-74.0620023250262!3d4.793836395191299!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x8e3f9c6f2a2f2a2f%3A0x6b2b2b2b2b2b2b2b!2sCra.%203%20%23136%20Sur-42%2C%20Bogot%C3%A1!5e0!3m2!1ses!2sco!4v1719280918000!5m2!1ses!2sco" <%-- Mapa corregido --%>
                width="600"
                height="450"
                style="border: 0"
                allowfullscreen=""
                loading="lazy"
                referrerpolicy="no-referrer-when-downgrade"
              ></iframe>
            </div>
          </section>

          <section class="business-hours">
            <h3>Horario Comercial</h3>
            <ul>
              <li><strong>Lunes a Viernes:</strong> 8:00 AM - 6:00 PM</li>
              <li><strong>Sábados:</strong> 9:00 AM - 2:00 PM</li>
              <li><strong>Domingos:</strong> Cerrado</li>
              <li><strong>Festivos:</strong> Consultar</li>
            </ul>
          </section>

          <%-- <section class="contact-form-section"> --%> <%-- Elimina esta sección --%>
            <%-- <h2>Contáctanos</h2> --%>
            <%-- <form action="#" method="POST"> --%>
              <%-- <div class="form-group"> --%>
                <%-- <label for="name">Nombre:</label> --%>
                <%-- <input type="text" id="name" name="name" required /> --%>
              <%-- </div> --%>
              <%-- <div class="form-group"> --%>
                <%-- <label for="email">Correo Electrónico:</label> --%>
                <%-- <input type="email" id="email" name="email" required /> --%>
              <%-- </div> --%>
              <%-- <div class="form-group"> --%>
                <%-- <label for="message">Mensaje:</label> --%>
                <%-- <textarea id="message" name="message" required></textarea> --%>
              <%-- </div> --%>
              <%-- <button type="submit">Enviar Mensaje</button> --%>
            <%-- </form> --%>
          <%-- </section> --%>
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