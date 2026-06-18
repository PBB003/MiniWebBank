using Microsoft.EntityFrameworkCore;
using MiniWebBank.Models;

namespace MiniWebBank.Data;

public class BankContext : DbContext 
{
    public DbSet<BankAccount> Accounts { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    public BankContext(DbContextOptions<BankContext> options) : base(options)
    {
    }
}