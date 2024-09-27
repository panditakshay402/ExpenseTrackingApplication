using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class BudgetCategoryTransactionCategory
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("BudgetCategory")]
    public int BudgetCategoryId { get; set; }
    public BudgetCategory BudgetCategory { get; set; }
    
    public string TransactionCategory { get; set; }
}
