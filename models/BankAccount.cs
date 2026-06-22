namespace MiniWebBank.Models;

public class BankAccount : IBankAccount
{
    public int Id { get; set; }
    public string Pin {get; set; }
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; protected set; }
    public string Role { get; set; }

    protected List<Transaction> allTransactions = new List<Transaction>();

    public BankAccount() { }

    public BankAccount(string newAccountNumber, string newOwnerName, decimal initialBalance, string pin, string role = "Client") 
    {
        AccountNumber = newAccountNumber;
        OwnerName = newOwnerName;
        if (initialBalance > 0) 
        {
            MakeDeposit(initialBalance, "Initial Balance");
        };
        Pin = pin;
        Role = role;
    }

    public virtual void MakeDeposit(decimal amount, string note) {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be positive");
        }
            Balance += amount;
            Transaction t = new Transaction(amount, DateTime.Now, note);
            allTransactions.Add(t);
    }

    public virtual void MakeWithdrawal(decimal amount, string note) {
        if (amount <= 0) {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be positive");
        }
        if (amount > Balance) {
            throw new InvalidOperationException("The amount is bigger than the balance");
        }
            Balance -= amount;
            Transaction t = new Transaction(-amount, DateTime.Now, note);
            allTransactions.Add(t);
    }

    public string GetAccountHistory() 
    {
        string report = "Date\t\t\t\tAmount\t\tNote\n";
        foreach (Transaction t in allTransactions) 
        {
            report += $"{t.Date}\t\t{t.Amount}\t\t{t.Note}\n";
        }

        return report;
    }

    public void ChangeOwnerName(string newOwnerName)
    {
        if (string.IsNullOrWhiteSpace(newOwnerName))
        {
            throw new ArgumentNullException(nameof(newOwnerName), "Owner name cannot be null");
        }

        OwnerName = newOwnerName;
    }

    public void Transfer(BankAccount destinationAccount, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be positive");
        }
        if (amount > Balance) 
        {
            throw new InvalidOperationException("The amount is bigger than the balance");
        }
        if (destinationAccount == null) 
        {
            throw new ArgumentNullException("The destination account cannot be null");
        }

        MakeWithdrawal(amount, $"Transfer to {destinationAccount.AccountNumber}");
        destinationAccount.MakeDeposit(amount, $"Received from {AccountNumber}");
    }

    public void SaveHistoryToFile()
    {
        string fileName = $"{AccountNumber}_history.txt";
        File.WriteAllText(fileName, GetAccountHistory());
    }

    public decimal GetTotalDeposit()
    {
        return allTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
    }
}