using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class EditIncomeViewModel
{
    public int? Id { get; set; }
    public string? Source { get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public IncomeCategory Category { get; set; }
    public string? Description { get; set; }
    public int BudgetId { get; set; }
}