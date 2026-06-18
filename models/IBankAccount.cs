public interface IBankAccount 
{
    void MakeDeposit(decimal amount, string note);
    void MakeWithdrawal(decimal amount, string note);
    string GetAccountHistory();
}