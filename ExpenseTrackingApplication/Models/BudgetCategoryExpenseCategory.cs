using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class BudgetCategoryExpenseCategory
{
    [Key]
    public int Id { get; init; }
    
    [ForeignKey("BudgetCategory")]
    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    
    public string ExpenseCategory { get; set; }
}
