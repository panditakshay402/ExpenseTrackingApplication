using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int Id { get; init; }
    
    [Required (ErrorMessage = "Name is required.")]
    [StringLength(25, ErrorMessage = "The Budget name must be at most 25 characters long.")]
    public string Name { get; set; } = "Budget";
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal Balance { get; set; }
    
    public DateTime CreatedDate  { get; set; } = DateTime.Now;
 
    [ForeignKey("AppUser")]
    [MaxLength(450)]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
    // Navigation properties
    public ICollection<Expense> Transactions { get; init; } = new List<Expense>();
    public ICollection<Income> Incomes { get; init; } = new List<Income>();
    public ICollection<Bill> Bills { get; init; } = new List<Bill>();
    public ICollection<BudgetCategory> BudgetCategories { get; init; } = new List<BudgetCategory>();
    public ICollection<Report>? Reports { get; init; } = new List<Report>();
}