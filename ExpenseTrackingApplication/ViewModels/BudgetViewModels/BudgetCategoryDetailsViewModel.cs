using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetCategoryDetailsViewModel
{
    public BudgetCategory BudgetCategory { get; set; }
    public IEnumerable<Expense> CategoryExpenses { get; set; }
}

