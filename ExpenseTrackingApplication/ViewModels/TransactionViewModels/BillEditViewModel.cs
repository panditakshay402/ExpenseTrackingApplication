using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels.TransactionViewModels;

public class BillEditViewModel
{
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public BillFrequency Frequency { get; set; }
    public int BudgetId { get; set; }
}