async function loadAccounts() {
    try {
        const response = await fetch('/accounts', { credentials: 'include' });
        
        if (response.status === 401 || response.status === 403) {
            alert('Acceso Denegado: Inicia sesión como Admin en el portal principal (index.html) primero.');
            return;
        }

        const accounts = await response.json();
        const list = document.getElementById('accounts-list');
        list.innerHTML = '';
        accounts.forEach(acc => {
            const li = document.createElement('li');
            li.innerHTML = `<strong>${acc.ownerName}</strong> (Cuenta: ${acc.accountNumber})<br/>Rol: ${acc.role} - Saldo: $${acc.balance.toFixed(2)}`;
            list.appendChild(li);
        });
    } catch (e) {
        console.error("Error cargando cuentas", e);
    }
}

// Crear nueva cuenta (Solo Admin)
document.getElementById('create-account-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    const number = document.getElementById('acc-number').value;
    const name = document.getElementById('acc-name').value;
    const balance = document.getElementById('acc-balance').value;
    const pin = document.getElementById('acc-pin').value;
    const role = document.getElementById('acc-role').value;

    const response = await fetch(`/accounts?number=${number}&name=${name}&initialBalance=${balance}&pin=${pin}&role=${role}`, {
        method: 'POST',
        credentials: 'include'
    });

    if (response.ok) {
        document.getElementById('create-account-form').reset();
        loadAccounts();
    } else {
        alert("Error al crear cuenta. ¿Acaso no eres Admin?");
    }
});

// Cargar cuentas al iniciar
loadAccounts();
