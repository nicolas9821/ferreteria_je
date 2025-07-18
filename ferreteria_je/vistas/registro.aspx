<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="registro.aspx.cs" Inherits="ferreteria_je.registro" %>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Registrarse FERRELECTRICOS</title>
  <link rel="stylesheet" href="../estilos/registro1.css">
  <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet">
  <link href="https://fonts.googleapis.com/css2?family=Forum&display=swap" rel="stylesheet">
</head>
<body>
<div class="container">
  <div class="left-container">
    <img src="../imagenes/Logo JE.jpg" alt="Logo de FERRELECTRICOS" class="logo">
    <p class="description">
      Somos FERRELECTRICOS: Expertos en soluciones eléctricas y ferretería para tus proyectos.
      ¡Bienvenido a la comunidad!
    </p>
  </div>
  <div class="right">
    <h2>Crear Cuenta Nueva</h2>
    <form id="form1" runat="server"> <%-- IMPORTANT: Changed to runat="server" to enable code-behind interaction --%>
      <asp:Label ID="lblMensaje" runat="server" CssClass="message" ForeColor="Red"></asp:Label><br /> <%-- Label for server-side messages --%>

      <label for="txtName">Nombre Completo</label>
      <asp:TextBox ID="txtName" runat="server" CssClass="input" placeholder="Tu nombre" required="required"></asp:TextBox>

      <label for="txtEmail">Correo</label>
      <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="Tu correo" TextMode="Email" required="required"></asp:TextBox>

      <label for="txtNewPassword">Nueva Contraseña</label>
      <asp:TextBox ID="txtNewPassword" runat="server" CssClass="input" TextMode="Password" placeholder="Nueva contraseña" required="required"></asp:TextBox>

      <label for="txtConfirmPassword">Confirmación de Contraseña</label>
      <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="input" TextMode="Password" placeholder="Confirma tu contraseña" required="required"></asp:TextBox>

      <label for="ddlRol">Rol</label>
<asp:DropDownList ID="ddlRol" runat="server" CssClass="input" AppendDataBoundItems="true">
    <asp:ListItem Text="Selecciona tu rol" Value="" />
    <asp:ListItem Text="Administrador" Value="admin" />
    <asp:ListItem Text="Cajero" Value="cajero" />
    <asp:ListItem Text="Auxiliar" Value="auxiliar" />
</asp:DropDownList>
          <%-- Add other roles if needed for registration, e.g., <asp:ListItem Text="Cajero" Value="cajero" /> --%>
      

      <asp:Button ID="btnConfirm" runat="server" Text="Confirmar" OnClick="btnConfirm_Click" CssClass="button" /> <%-- Changed to ASP.NET Button with OnClick event --%>

      <%-- Keep client-side messages for initial validation hints (their display will be controlled by JS) --%>
      <p id="emailMessage" class="message">Ingresa un correo electrónico válido.</p>
      <p id="passwordMessage" class="message">Tu contraseña debe tener al menos 8 caracteres, un número y un símbolo especial.</p>
    </form>

    <div class="links">
      <a href="login.aspx">← Volver a inicio</a>
    </div>
  </div>
</div>

<script>
    // Update JavaScript IDs to match ASP.NET control ClientIDs
    const emailInput = document.getElementById('<%= txtEmail.ClientID %>');
    const nameInput = document.getElementById('<%= txtName.ClientID %>');
    const newPasswordInput = document.getElementById('<%= txtNewPassword.ClientID %>');
  const confirmPasswordInput = document.getElementById('<%= txtConfirmPassword.ClientID %>');
  const ddlRol = document.getElementById('<%= ddlRol.ClientID %>'); // Get the Rol dropdown
  const confirmButton = document.getElementById('<%= btnConfirm.ClientID %>');
    const passwordMessage = document.getElementById('passwordMessage');
    const emailMessage = document.getElementById('emailMessage');

    function validateForm() {
        const nameValue = nameInput.value.trim();
        const emailValue = emailInput.value.trim();
        const password = newPasswordInput.value;
        const confirm = confirmPasswordInput.value;
        const rolValue = ddlRol.value; // Get the selected rol value

        const hasMinLength = password.length >= 8;
        const hasNumber = /\d/.test(password);
        const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
        const passwordsMatch = password === confirm;
        const validEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailValue);

        const passwordValid = hasMinLength && hasNumber && hasSpecialChar;

        // Check if all required fields have some value, including the selected rol
        const allFieldsFilled = nameValue !== '' && emailValue !== '' && password !== '' && confirm !== '' && rolValue !== '';

        // Display/hide password message
        if (!passwordValid && password.length > 0) {
            passwordMessage.style.display = 'block';
        } else {
            passwordMessage.style.display = 'none';
        }

        // Display/hide email message
        if (!validEmail && emailValue.length > 0) {
            emailMessage.style.display = 'block';
        } else {
            emailMessage.style.display = 'none';
        }

        // Enable/disable button based on all validations
        if (allFieldsFilled && passwordValid && passwordsMatch && validEmail) {
            confirmButton.disabled = false;
        } else {
            confirmButton.disabled = true;
        }
    }

    // Listen for input changes to validate in real-time
    nameInput.addEventListener('input', validateForm);
    emailInput.addEventListener('input', validateForm);
    newPasswordInput.addEventListener('input', validateForm);
    confirmPasswordInput.addEventListener('input', validateForm);
    ddlRol.addEventListener('change', validateForm); // Listen for change on the dropdown

    // Initial validation call to set button state on page load
    validateForm();

</script>
</body>
</html>