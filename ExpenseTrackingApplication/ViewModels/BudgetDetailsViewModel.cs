using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetDetailsViewModel
{
    public Budget Budget { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    public IEnumerable<Income> Incomes { get; set; }
    public IEnumerable<Budget> AllBudgets { get; set; }
    public IEnumerable<BudgetCategory> BudgetCategories { get; set; }
    public decimal TotalTransactionAmount => Transactions.Sum(t => t.Amount);
    public decimal TotalIncomeAmount => Incomes.Sum(i => i.Amount);
    public SelectList BudgetSelectList { get; set; }
}
