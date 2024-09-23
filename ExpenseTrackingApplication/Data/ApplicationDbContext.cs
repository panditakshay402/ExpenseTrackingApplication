using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Security> Securities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Budgets)
            .WithOne(b => b.AppUser)
            .HasForeignKey(b => b.AppUserId);

        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Transactions)
            .WithOne(t => t.Budget)
            .HasForeignKey(t => t.BudgetId);
        
        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Incomes)
            .WithOne(t => t.Budget)
            .HasForeignKey(t => t.BudgetId);
        
        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Bills)
            .WithOne(t => t.Budget)
            .HasForeignKey(t => t.BudgetId);

        modelBuilder.Entity<Budget>()
            .HasMany(b => b.BudgetCategories)
            .WithOne(bc => bc.Budget)
            .HasForeignKey(bc => bc.BudgetId);
        
        // Configure precision and scale for decimal properties
        modelBuilder.Entity<Budget>()
            .Property(b => b.Balance)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<BudgetCategory>()
            .Property(bc => bc.CurrentSpending)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<BudgetCategory>()
            .Property(bc => bc.Limit)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Income>()
            .Property(i => i.Amount)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<Bill>()
            .Property(i => i.Amount)
            .HasColumnType("decimal(18,2)");
        
    }
    
}