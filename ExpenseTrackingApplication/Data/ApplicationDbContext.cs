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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Security> Securities { get; set; }
    public DbSet<Report> Reports { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Budgets)
            .WithOne(b => b.AppUser)
            .HasForeignKey(b => b.AppUserId);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Reports)
            .WithOne(r => r.AppUser)
            .HasForeignKey(r => r.AppUserId);

        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Transactions)
            .WithOne(t => t.Budget)
            .HasForeignKey(t => t.BudgetId);
        
        modelBuilder.Entity<Budget>()
            .HasMany(b => b.Incomes)
            .WithOne(t => t.Budget)
            .HasForeignKey(t => t.BudgetId);

        modelBuilder.Entity<Budget>()
            .HasMany(b => b.BudgetCategories)
            .WithOne(bc => bc.Budget)
            .HasForeignKey(bc => bc.BudgetId);
        
    }
    
}