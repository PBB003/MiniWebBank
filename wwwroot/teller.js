async function executeOperation(type) {
    const number = document.getElementById('client-number').value;
    const amount = document.getElementById('op-amount').value;
    const note = document.getElementById('op-note').value;

    if (!number || !amount) {
        alert("Please enter account number and amount.");
        return;
    }

    const response = await fetch(`/accounts/${number}/${type}?amount=${amount}&note=${note}`, {
        method: 'POST',
        credentials: 'include'
    });

    if (response.ok) {
        alert(`${type === 'deposit' ? 'Deposit' : 'Withdrawal'} successful.`);
        document.getElementById('cashier-form').reset();
    } else {
        alert("Operation failed. Verify account, balance or permissions.");
    }
}

document.getElementById('btn-deposit').addEventListener('click', () => executeOperation('deposit'));
document.getElementById('btn-withdraw').addEventListener('click', () => executeOperation('withdraw'));
