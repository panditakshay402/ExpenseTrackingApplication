using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.TransactionViewModels;

namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetDetailsViewModel
{
    public Budget Budget { get; set; }
    public IEnumerable<CombinedEntryViewModel> CombinedEntries { get; set; }
    public IEnumerable<Bill> Bills { get; set; }
    public IEnumerable<Budget> AllBudgets { get; set; }
    public IEnumerable<BudgetCategory> BudgetCategories { get; set; }
    public decimal TotalExpenseAmount { get; set; }
    public decimal TotalIncomeAmount { get; set; }
}
