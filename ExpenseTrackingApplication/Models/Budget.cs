using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int Id { get; set; }
    
    [Required (ErrorMessage = "Name is required.")]
    [MaxLength(100)]
    public string Name { get; set; } = "Budget";
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal Balance { get; set; }
 
    [ForeignKey("AppUser")]
    [MaxLength(450)]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();
    public ICollection<Report>? Reports { get; set; } = new List<Report>();

}