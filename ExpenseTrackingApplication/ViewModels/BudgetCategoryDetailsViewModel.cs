using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryDetailsViewModel
{
    public BudgetCategory BudgetCategory { get; set; }
    public IEnumerable<Transaction> CategoryTransactions { get; set; }
}

