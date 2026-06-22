// app.js: Exclusivo para la página de Login (index.html)

document.getElementById('login-form').addEventListener('submit', async (e) => { 
    e.preventDefault();

    const number = document.getElementById('login-number').value;
    const pin = document.getElementById('login-pin').value;

    const response = await fetch(`/login?number=${number}&pin=${pin}`, {
        method: 'POST'
    });

    if (response.ok) {
        const data = await response.json(); // Leemos el Rol
        
        if (data.role === "Admin") {
            window.location.href = "/admin.html"; // Redirigir al CRM
        } else if (data.role === "Cajero") {
            window.location.href = "/cajero.html"; // Redirigir a Ventanilla
        } else {
            window.location.href = "/dashboard.html"; // Redirigir al Home Banking
        }
    } else {
        alert("Número de cuenta o PIN incorrectos. Acceso Denegado.");
    }
});