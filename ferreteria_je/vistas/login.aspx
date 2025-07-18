<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ferreteria_je.login" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Inicio Sesión</title>
    <link rel="stylesheet" href="../estilos/login.css" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Forum&display=swap" rel="stylesheet" />
</head>
<body class="login-page">
    <form id="form1" runat="server">
        <div class="container">
            <div class="left-container">
                <img src="../imagenes/Logo JE.jpg" alt="Logo de FERRELECTRICOS" class="logo" />
                <p class="description">
                    Somos FERRELECTRICOS: Expertos en soluciones eléctricas y ferretería para tus proyectos.
                </p>
            </div>
            <div class="right">
                <h2>Inicia Sesión</h2>

                <asp:Label ID="lblMensaje" runat="server" CssClass="message" ForeColor="Red"></asp:Label><br />

                <label for="email">Correo electrónico</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Tu correo" TextMode="Email" required></asp:TextBox>

                <label for="newPassword">Contraseña</label>
                <div class="password-input-wrapper">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="input" placeholder="Ingresa contraseña" TextMode="Password" required></asp:TextBox>
                    <span class="toggle-password" id="togglePassword">👁️</span>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Confirmar" OnClick="btnLogin_Click" CssClass="btn" />

                <div class="links">
                    <a href="dashboard.aspx">← Volver</a>
                </div>
                <div class="links">
                
                    <a href="NUEVACONTRASEÑA.aspx">Olvidé mi contraseña</a>
                </div>
                <div class="links">
                    <span class="no-account-text">¿No tienes cuenta aún?</span>
                    <a href="registro.aspx">Registrarse</a>
                </div>
            </div>
        </div>
    </form>

    <script>
        const togglePassword = document.getElementById('togglePassword');
        const passwordInput = document.getElementById('<%= txtPassword.ClientID %>');
        togglePassword.addEventListener('click', function () {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
        });
    </script>
</body>
</html>
