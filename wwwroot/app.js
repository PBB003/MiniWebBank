document.addEventListener("DOMContentLoaded", () => { loadAccounts(); });

async function loadAccounts() {
    try {
        const response = await
        fetch('/accounts');
        const accounts = await response.json();

        const listDiv = document.getElementById('accounts-list');
        listDiv.innerHTML = '';

        if (accounts.length === 0) {
            listDiv.innerHTML = '<p style="color:gray;">Aún no tienes cuentas creadas.</p>';
            return;
        }

        accounts.forEach(acc => {
            const accDiv = document.createElement('div');
            accDiv.className = 'account-item';
            accDiv.innerHTML = `
            <div class="account-details">
                <h3>${acc.ownerName}</h3>
                <p>Cuenta: ${acc.accountNumber || "SIn Numero"}</p>
            </div>
            <div class="account-balance">
            <div class="amount">$${acc.balance.toFixed(2)}</div>
            </div> 
            `;
            listDiv.appendChild(accDiv);
        });
    } catch (error) {
        console.log("Error al cargar cuentas:", error);
    }
}

document.getElementById('create-account-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    const number = document.getElementById('acc-number').value;
    const name = document.getElementById('acc-name').value;
    const balance = document.getElementById('acc-balance').value;
    const pin = document.getElementById('acc-pin').value;

    await fetch(`/accounts?number=${number}&name=${name}&initialBalance=${balance}&pin=${pin}`, {
        method: 'POST'
    });

    document.getElementById('create-account-form').reset();
    loadAccounts();
});

document.getElementById('make-deposit').addEventListener('submit', async (e) => {
    e.preventDefault();

    const number = document.getElementById('deposit-acc-number').value;
    const amount = document.getElementById('deposit-amount').value;
    const note = document.getElementById('note').value;

    await fetch(`/accounts/${number}/deposit?amount=${amount}&note=${note}`, {
        method: 'POST',
        credentials: 'include'
    });

    document.getElementById('make-deposit').reset();
    loadAccounts();
});

document.getElementById('login-form').addEventListener('submit', async (e) => { 
    e.preventDefault();

    const number = document.getElementById('login-number').value;
    const pin = document.getElementById('login-pin').value;

    const response = await fetch(`/login?number=${number}&pin=${pin}`, {
        method: 'POST'
    });

    if (response.ok) {
        alert("¡Cookie recibido! Ya puedes hacer depósitos.");
    } else {
        alert("Número de cuenta o PIN incorrectos.");
    }
});