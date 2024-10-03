using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetCategoryDetailsViewModel
{
    public BudgetCategory BudgetCategory { get; set; }
    public IEnumerable<Transaction> CategoryTransactions { get; set; }
}

