async function loadMyData() {
    const response = await fetch('/accounts/me', { credentials: 'include' });
    
    if (response.ok) {
        const data = await response.json();
        document.getElementById('current-balance').innerText = `$${data.balance.toFixed(2)}`;
    } else {
        alert("Session expired.");
        window.location.href = '/index.html';
    }
}

document.getElementById('make-transfer').addEventListener('submit', async (e) => {
    e.preventDefault();
    const target = document.getElementById('target-number').value;
    const amount = document.getElementById('transfer-amount').value;

    const response = await fetch(`/accounts/transfer?targetNumber=${target}&amount=${amount}`, {
        method: 'POST',
        credentials: 'include'
    });

    if (response.ok) {
        alert("Transfer successful.");
        document.getElementById('make-transfer').reset();
        loadMyData(); 
    } else {
        alert("Transfer failed. Please check the target account and your available balance.");
    }
});

loadMyData();
