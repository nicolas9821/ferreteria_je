<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NUEVACONTRASEÑA.aspx.cs" Inherits="ferreteria_je.NUEVACONTRASEÑA" %>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Restablecer Contraseña</title>
  <link rel="stylesheet" href="../estilos/nueva_contraseña.css">
  <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap" rel="stylesheet">
  <link href="https://fonts.googleapis.com/css2?family=Forum&display=swap" rel="stylesheet">
</head>
<body>
<div class="container">
  <div class="left-container">
    <img src="../imagenes/Logo JE.jpg" alt="Logo de FERRELECTRICOS" class="logo">
    <p class="description">
      Somos FERRELECTRICOS: Expertos en soluciones eléctricas y ferretería para tus proyectos.
    </p>
  </div>
  <div class="right">
    <h2>Restablecer Contraseña</h2>
    <form id="resetForm">
      <label for="email">Correo electrónico</label>
      <input type="email" id="email" placeholder="Tu correo" required>

      <label for="newPassword">Nueva Contraseña</label>
      <input type="password" id="newPassword" placeholder="Nueva contraseña" required>

      <label for="confirmPassword">Confirmación de Contraseña</label>
      <input type="password" id="confirmPassword" placeholder="Confirma tu contraseña" required>

      <button type="submit" id="confirmBtn" disabled>Confirmar</button>

      <p id="emailMessage" class="message">Ingresa un correo electrónico válido.</p>
      <p id="passwordMessage" class="message">Tu contraseña debe tener al menos 8 caracteres, un número y un símbolo especial.</p>
    </form>

    <div class="links">
      <a href="login.aspx">← Volver a inicio</a>
    </div>
  </div>
</div>

<script>
  const email = document.getElementById('email');
  const newPassword = document.getElementById('newPassword');
  const confirmPassword = document.getElementById('confirmPassword');
  const confirmBtn = document.getElementById('confirmBtn');
  const passwordMessage = document.getElementById('passwordMessage');
  const emailMessage = document.getElementById('emailMessage');
  const resetForm = document.getElementById('resetForm');
  // La variable backToHome no se usa en el JS que me proporcionaste, pero la mantengo si la planeas usar
  // const backToHome = document.getElementById('backToHome');

  function validatePassword() {
    const password = newPassword.value;
    const confirm = confirmPassword.value;
    const emailValue = email.value;

    const hasMinLength = password.length >= 8;
    const hasNumber = /\d/.test(password);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    const passwordsMatch = password === confirm;
    const validEmail = emailValue.includes('@');

    const passwordValid = hasMinLength && hasNumber && hasSpecialChar;

    // Mostrar u ocultar mensaje de contraseña
    if (!passwordValid && password.length > 0) { // Añadido password.length > 0 para no mostrar al inicio
      passwordMessage.style.display = 'block';
    } else {
      passwordMessage.style.display = 'none';
    }

    // Mostrar u ocultar mensaje de correo
    if (!validEmail && emailValue.length > 0) { // Añadido emailValue.length > 0 para no mostrar al inicio
      emailMessage.style.display = 'block';
    } else {
      emailMessage.style.display = 'none';
    }

    // Habilitar botón solo si todo es válido y las contraseñas coinciden
    if (passwordValid && passwordsMatch && validEmail) {
      confirmBtn.disabled = false;
    } else {
      confirmBtn.disabled = true;
    }
  }

  newPassword.addEventListener('input', validatePassword);
  confirmPassword.addEventListener('input', validatePassword);
  email.addEventListener('input', validatePassword);

  resetForm.addEventListener('submit', function(event) {
    event.preventDefault();
    // Aquí puedes añadir lógica para enviar el correo o actualizar la contraseña en un backend real.
    // Por ahora, solo muestra el mensaje de éxito.
    resetForm.innerHTML = '<h3>¡Contraseña actualizada exitosamente!</h3>';
    // Si backToHome se refiere a un elemento con esa ID y lo quieres mostrar:
    // if (backToHome) {
    //   backToHome.classList.remove('hidden');
    // }
  });
</script>
</body>
</html>
