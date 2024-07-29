using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal? Amount { get; set; } = 0;
 
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();

}