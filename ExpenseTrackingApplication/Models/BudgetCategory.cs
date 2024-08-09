using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class BudgetCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }  = "Budget Category";

    [Required]
    public BudgetCategoryType Type { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Current spending must be a positive number.")]
    public decimal CurrentSpending { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
    public decimal Limit { get; set; }
    
    public decimal RemainingBalance => Limit - CurrentSpending;
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}