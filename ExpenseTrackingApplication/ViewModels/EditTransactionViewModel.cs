using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class EditTransactionViewModel
{
    public int? Id { get; set; }
    public TransactionCategory Category { get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? AppUserId {  get; set; }
}