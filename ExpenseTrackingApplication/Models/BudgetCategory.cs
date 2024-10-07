using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class BudgetCategory
{
    [Key]
    public int Id { get; init; }

    [Required]
    [StringLength(50, ErrorMessage = "The Budget Category name must be at most 50 characters long.")]
    public string Name { get; set; }  = "Budget Category";
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
    public decimal Limit { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Current spending must be a positive number.")]
    public decimal CurrentSpending { get; set; }
    public decimal RemainingBalance => Limit - CurrentSpending;
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
    
    public ICollection<BudgetCategoryExpenseCategory> BudgetCategoryExpenseCategories { get; init; } = new List<BudgetCategoryExpenseCategory>();
}