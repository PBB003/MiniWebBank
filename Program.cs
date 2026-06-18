using Microsoft.EntityFrameworkCore;
using MiniWebBank.Data;
using MiniWebBank.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BankContext> (options => options.UseSqlite("Data Source=banco.db"));
var app = builder.Build();

app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = 
    scope.ServiceProvider.GetRequiredService<BankContext>();
        db.Database.EnsureCreated();
}

app.MapGet("/", () => "Bienvenido al Mini Banco Web!");

app.MapGet("/accounts", async (BankContext db) => {
    return await db.Accounts.ToListAsync();
});

app.MapPost("/accounts", async (BankContext db, string number, string name, decimal initialBalance) => 
{
    var account = new BankAccount(number, name, initialBalance);
    db.Accounts.Add(account);
    await db.SaveChangesAsync();
    return Results.Ok(account);
});

app.MapPost("/accounts/{id}/withdraw", async (BankContext db, int id, decimal amount, string note) => 
{
    var account = await db.Accounts.FindAsync(id);
    if (account == null)
    {
        return Results.NotFound("Cuenta no encontrada");
    }

    account.MakeWithdrawal(amount, note);

    await db.SaveChangesAsync();

    return Results.Ok(account);
});

app.MapPost("/accounts/{number}/deposit", async (BankContext db, string number, decimal amount, string note) =>
{
    var account = await db.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == number);
    if (account == null)
    {
        return Results.NotFound("Cuenta no encontrada");
    }

    account.MakeDeposit(amount, note);

    await db.SaveChangesAsync();

    return Results.Ok(account);
});

app.Run();