namespace ExpenseTrackingApplication.ViewModels;

public class EditBudgetViewModel
{
    public int? Id { get; set; }
    public decimal Balance { get; set; }
    public string? AppUserId { get; set; }
}