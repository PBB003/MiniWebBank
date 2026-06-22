document.getElementById('login-form').addEventListener('submit', async (e) => { 
    e.preventDefault();

    const number = document.getElementById('login-number').value;
    const pin = document.getElementById('login-pin').value;

    const response = await fetch(`/login?number=${number}&pin=${pin}`, {
        method: 'POST'
    });

    if (response.ok) {
        const data = await response.json(); 
        
        if (data.role === "Admin") {
            window.location.href = "/admin.html"; // Redirect to CRM
        } else if (data.role === "Teller") {
            window.location.href = "/teller.html"; // Redirect to Teller UI
        } else {
            window.location.href = "/dashboard.html"; // Redirect to Home Banking
        }
    } else {
        alert("Incorrect account number or PIN. Access Denied.");
    }
});