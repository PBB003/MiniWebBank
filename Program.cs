using Microsoft.EntityFrameworkCore;
using MiniWebBank.Data;
using MiniWebBank.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("NeonDB");
builder.Services.AddDbContext<BankContext>(options => options.UseNpgsql(connectionString));



// --- SECURITY CONFIGURATION (COOKIES) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // Return 401 instead of redirecting
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
// --------------------------------------------

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BankContext>();
    
    db.Database.EnsureCreated();
    
    if (!db.Accounts.Any()) 
    {
        var firstAdmin = new BankAccount("admin", "Bank Manager", 1000000, "1234", "Admin");
        var firstTeller = new BankAccount("teller", "Front Desk", 0, "1234", "Teller");
        db.Accounts.Add(firstAdmin);
        db.Accounts.Add(firstTeller);
        db.SaveChanges();
    }
}

app.MapGet("/", () => "Welcome to MiniWebBank!");

app.MapGet("/accounts", async (BankContext db) => {
    return await db.Accounts.ToListAsync();
}).RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapGet("/accounts/me", async (BankContext db, HttpContext http) => { 
    var myNumber = http.User.Identity.Name;
    var myAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == myNumber);

    if (myAccount == null) {
        return Results.NotFound();
    }
    return Results.Ok(new { balance = myAccount.Balance, role = myAccount.Role });
}).RequireAuthorization();

app.MapPost("/accounts", async (BankContext db, string number, string name, decimal initialBalance, string pin, string role = "Client") => 
{
    var account = new BankAccount(number, name, initialBalance, pin, role);
    db.Accounts.Add(account);
    await db.SaveChangesAsync();
    return Results.Ok(account);
}).RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapPost("/accounts/{id}/withdraw", async (BankContext db, int id, decimal amount, string note) => 
{
    var account = await db.Accounts.FindAsync(id);
    if (account == null)
    {
        return Results.NotFound("Account not found");
    }

    account.MakeWithdrawal(amount, note);

    await db.SaveChangesAsync();

    return Results.Ok(account);
}).RequireAuthorization(policy => policy.RequireRole("Admin", "Teller"));

app.MapPost("/accounts/{number}/deposit", async (BankContext db, string number, decimal amount, string note) =>
{
    var account = await db.Accounts.FirstOrDefaultAsync(u => u.AccountNumber == number);
    if (account == null)
    {
        return Results.NotFound("Account not found");
    }

    account.MakeDeposit(amount, note);

    await db.SaveChangesAsync();

    return Results.Ok(account);
}).RequireAuthorization(policy => policy.RequireRole("Admin", "Teller"));

app.MapPost("/login", async (BankContext db, HttpContext http, string number, string pin) => {
    var account = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == number && a.Pin == pin);

    if (account == null) {
        return Results.Unauthorized();
    } 

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, account.AccountNumber), new Claim(ClaimTypes.Role, account.Role)};
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    return Results.Ok(new { message = "Login successful", role = account.Role });
});

app.MapPost("/accounts/transfer", async (BankContext db, HttpContext http, string targetNumber, decimal amount) => {
    var myNumber = http.User.Identity.Name;
    
    var sourceAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == myNumber);
    var targetAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == targetNumber);
    
    if (sourceAccount == null || targetAccount == null) return Results.NotFound("Account not found.");
    if (sourceAccount.Balance < amount) return Results.BadRequest("Insufficient funds.");

    using var transaction = await db.Database.BeginTransactionAsync();
    try 
    {
        // Withdraw
        sourceAccount.MakeWithdrawal(amount, $"Transfer to {targetNumber}");
        
        // Deposit
        targetAccount.MakeDeposit(amount, $"Transfer from {myNumber}");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return Results.Ok("Transfer successful.");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return Results.BadRequest("Transfer failed.");
    }
}).RequireAuthorization();

app.MapGet("/logout", async (HttpContext http) => {
    await http.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/index.html");
});

app.Run();