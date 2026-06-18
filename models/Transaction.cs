public class Transaction 
{
    public int Id { get; set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Note {get; private set;}

    public Transaction() { }

    public Transaction (decimal amount, DateTime date, string note)
    {
        Amount = amount;
        Date = date;
        Note = note;
    }
}