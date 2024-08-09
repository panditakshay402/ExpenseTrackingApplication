using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BudgetCategoryType Type { get; set; }
    public decimal CurrentSpending { get; set; }
    public decimal Limit { get; set; }
    public int BudgetId { get; set; }
}