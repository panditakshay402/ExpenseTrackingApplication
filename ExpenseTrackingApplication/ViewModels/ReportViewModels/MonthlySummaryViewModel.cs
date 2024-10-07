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
    public List<Expense> Expenses { get; set; } = new List<Expense>();
    public List<Income> Incomes { get; set; } = new List<Income>();
}