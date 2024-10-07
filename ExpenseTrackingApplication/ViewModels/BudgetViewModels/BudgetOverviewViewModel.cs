namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetOverviewViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpense { get; set; }
    public int MonthlyTotalTransactions { get; set; }
    public DateTime CreatedDate { get; set; }
}