namespace ExpenseTrackingApplication.ViewModels.TransactionViewModels;

public class CombinedEntryViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Type { get; set; } // "Expense" or "Income"
    public string? RecipientOrSource { get; set; }
    public decimal Amount { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
