using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BudgetCategoryType Type { get; set; }
    public decimal CurrentSpending { get; set; }
    public decimal Limit { get; set; }
    public decimal RemainingBalance { get; set; }
    
    public IEnumerable<Transaction>? Transactions { get; set; }
    public IEnumerable<Income>? Incomes { get; set; }
}
