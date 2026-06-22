async function executeOperation(type) {
    const number = document.getElementById('client-number').value;
    const amount = document.getElementById('op-amount').value;
    const note = document.getElementById('op-note').value;

    if (!number || !amount) {
        alert("Completa la cuenta y el monto.");
        return;
    }

    const response = await fetch(`/accounts/${number}/${type}?amount=${amount}&note=${note}`, {
        method: 'POST',
        credentials: 'include'
    });

    if (response.ok) {
        alert(`Operación (${type === 'deposit' ? 'Depósito' : 'Retiro'}) exitosa.`);
        document.getElementById('cashier-form').reset();
    } else {
        alert("Error en la operación. Verifica la cuenta, saldo, o tus permisos.");
    }
}

document.getElementById('btn-deposit').addEventListener('click', () => executeOperation('deposit'));
document.getElementById('btn-withdraw').addEventListener('click', () => executeOperation('withdraw'));
