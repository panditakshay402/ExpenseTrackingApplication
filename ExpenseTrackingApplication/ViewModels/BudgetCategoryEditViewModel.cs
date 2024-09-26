using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Limit { get; set; }
}
