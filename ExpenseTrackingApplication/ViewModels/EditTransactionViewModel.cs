using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class EditTransactionViewModel
{
    public int? Id { get; set; }
    public string? Recipient { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionCategory Category { get; set; }
    public string? Description { get; set; }
    public int BudgetId { get; set; }
}