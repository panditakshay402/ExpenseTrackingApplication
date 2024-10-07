namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class AssignExpenseCategoriesViewModel
{
    public int BudgetCategoryId { get; set; }
    public List<string> AllExpenseCategories { get; set; }
    public List<string> SelectedCategories { get; set; }
}

