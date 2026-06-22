using Microsoft.EntityFrameworkCore;
using MiniWebBank.Data;
using MiniWebBank.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BankContext> (options => options.UseSqlite("Data Source=banco.db"));

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
}

app.MapGet("/", () => "Bienvenido al Mini Banco Web!");

app.MapGet("/accounts", async (BankContext db) => {
    return await db.Accounts.ToListAsync();
});

app.MapPost("/accounts", async (BankContext db, string number, string name, decimal initialBalance, string pin) => 
{
    var account = new BankAccount(number, name, initialBalance, pin);
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
}).RequireAuthorization();

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
}).RequireAuthorization();

app.MapPost("/login", async (BankContext db, HttpContext http, string number, string pin) => {
    var account = await
    db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == number && a.Pin == pin);

        if (account == null) {
            return Results.Unauthorized();
        } 

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, account.AccountNumber)};
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await
        http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Results.Ok("Login exitoso");
});

app.Run();