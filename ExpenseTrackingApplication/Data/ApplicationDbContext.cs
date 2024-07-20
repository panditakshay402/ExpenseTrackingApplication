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
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Security> Securities { get; set; }
    public DbSet<Report> Reports { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Transactions)
            .WithOne(t => t.AppUser)
            .HasForeignKey(t => t.AppUserId);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Reports)
            .WithOne(r => r.AppUser)
            .HasForeignKey(r => r.AppUserId);


    }
    
}