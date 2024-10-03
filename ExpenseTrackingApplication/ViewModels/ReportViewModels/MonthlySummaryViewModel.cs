using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels.ReportViewModels;

public class MonthlySummaryViewModel
{
    public string? BudgetName { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal NetSavings { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    public List<Income> Incomes { get; set; } = new List<Income>();
}