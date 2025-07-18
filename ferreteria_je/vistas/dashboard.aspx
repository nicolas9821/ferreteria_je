<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="ferreteria_je.dashboard" %>

<!DOCTYPE html>
<html lang="es" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>DISEÑO WEB I</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet" />

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"
          integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous" />

    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Pacifico&family=Quicksand:wght@300..700&display=swap" rel="stylesheet" />

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <link rel="stylesheet" href="../estilos/dashboard.css" />
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar" role="navigation" aria-label="Menú principal">
            <div class="logo">
                <a href="#"><img src="../imagenes/Logo JE.jpg" alt="Logo Ferretería JE" /></a>
            </div>
            <div class="nav-links">
                <a href="dashboard.aspx">Home</a>
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
            <article class="article">
                <section id="importancia-ferreteria" tabindex="-1">
                    <div class="article-content">
                        <h2>La importancia de las ferreterías en Colombia</h2>
                        <p>
                            Las ferreterías en Colombia constituyen un sector fundamental que sostiene gran parte de la economía local y nacional.
                        </p>
                        <p>
                            No solo abastecen materiales esenciales para la construcción y el mantenimiento del hogar, sino que también generan empleo, fomentan el emprendimiento y contribuyen al desarrollo de las comunidades.
                        </p>
                        <p>
                            Además, en zonas rurales y urbanas, las ferreterías actúan como centros de confianza para quienes buscan mejorar sus viviendas o emprender proyectos de obra civil. Su papel ha sido clave en la autoconstrucción, un fenómeno muy común en el país.
                        </p>
                        <p>
                            Apoyar al comercio ferretero local es apoyar el crecimiento del país desde la base, fortaleciendo la economía circular y la cadena productiva nacional.
                        </p>
                    </div>
                </section>
            </article>

            <aside class="sidebar" role="complementary" aria-label="Tabla de contenido">
                <h3>Tabla de contenido</h3>
                <ul>
                    <li><a href="#importancia-ferreteria">Importancia de una Ferretería</a></li>
                </ul>
            </aside>
        </main>

        <footer class="main-footer">
            <p>© 2025 Ferrelectricos JE - Todos los derechos reservados. ¡Bienvenidos a la ferretería!</p>
            <div class="social-links-footer">
                <a href="https://wa.me/573102522316" target="_blank" rel="noopener noreferrer" aria-label="WhatsApp de Ferrelectricos" class="social-icon-whatsapp">
                    <i class="bi bi-whatsapp icon-large"></i>
                </a>

                <a href="https://www.instagram.com/je_ferrelectricos/" target="_blank" rel="noopener noreferrer" aria-label="Instagram de Ferrelectricos" class="social-icon-instagram">
                    <i class="bi bi-instagram icon-large"></i>
                </a>

                <a href="mailto:Ferrelectricos@gmail.com" target="_blank" rel="noopener noreferrer" aria-label="Correo de Ferrelectricos" class="social-icon-gmail">
                    <i class="bi bi-envelope-fill icon-large"></i>
                </a>
            </div>
        </footer>
    </form>
</body>
</html>