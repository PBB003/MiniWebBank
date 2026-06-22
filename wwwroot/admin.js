async function loadAccounts() {
    try {
        const response = await fetch('/accounts', { credentials: 'include' });
        
        if (response.status === 401 || response.status === 403) {
            alert('Access Denied: Please sign in as Admin first.');
            return;
        }

        const accounts = await response.json();
        const list = document.getElementById('accounts-list');
        list.innerHTML = '';
        accounts.forEach(acc => {
            const li = document.createElement('li');
            li.innerHTML = `<strong>${acc.ownerName}</strong> (Account: ${acc.accountNumber})<br/>Role: ${acc.role} - Balance: $${acc.balance.toFixed(2)}`;
            list.appendChild(li);
        });
    } catch (e) {
        console.error("Error loading accounts", e);
    }
}

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
        alert("Error creating account. Are you sure you're an Admin?");
    }
});

loadAccounts();
