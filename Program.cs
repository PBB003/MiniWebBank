using Microsoft.EntityFrameworkCore;
using MiniWebBank.Data;
using MiniWebBank.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("NeonDB");
builder.Services.AddDbContext<BankContext>(options => options.UseNpgsql(connectionString));



// --- CONFIGURACIÓN DE SEGURIDAD (COOKIES) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // Devolver Error 401 en lugar de redirigir
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
    var db = 
    scope.ServiceProvider.GetRequiredService<BankContext>();
        db.Database.EnsureCreated();
        if (!db.Accounts.Any()) // Si la base de datos está vacía...
    {
        // Se crea una cuenta "admin", PIN "1234", Rol "Admin"
        var primerAdmin = new BankAccount("admin", "El Jefe", 1000000, "1234", "Admin");
        db.Accounts.Add(primerAdmin);
        db.SaveChanges();
    }
}

app.MapGet("/", () => "Bienvenido al Mini Banco Web!");

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
        return Results.NotFound("Cuenta no encontrada");
    }

    account.MakeWithdrawal(amount, note);

    await db.SaveChangesAsync();

    return Results.Ok(account);
}).RequireAuthorization(policy => policy.RequireRole("Admin", "Cajero"));

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
}).RequireAuthorization(policy => policy.RequireRole("Admin", "Cajero"));

app.MapPost("/login", async (BankContext db, HttpContext http, string number, string pin) => {
    var account = await
    db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == number && a.Pin == pin);

        if (account == null) {
            return Results.Unauthorized();
        } 

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, account.AccountNumber), new Claim(ClaimTypes.Role, account.Role)};
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Results.Ok(new { message = "Login exitoso", role = account.Role });
});

app.MapPost("/accounts/transfer", async (BankContext db, HttpContext http, string targetNumber, decimal amount) => {
    var myNumber = http.User.Identity.Name;
    
    var sourceAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == myNumber);
    var targetAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == targetNumber);
    if (sourceAccount == null || targetAccount == null) return Results.NotFound("Cuenta no encontrada.");
    if (sourceAccount.Balance < amount) return Results.BadRequest("Saldo insuficiente.");

    using var transaction = await db.Database.BeginTransactionAsync();
    try 
    {
        // Retirar
        sourceAccount.MakeWithdrawal(amount, $"Transferencia a {targetNumber}");
        
        // Depositar
        targetAccount.MakeDeposit(amount, $"Transferencia de {myNumber}");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return Results.Ok("Transferencia exitosa.");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return Results.BadRequest("Error en la transferencia.");
    }
}).RequireAuthorization();

app.MapGet("/logout", async (HttpContext http) => {
    await http.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/index.html");
});

app.Run();