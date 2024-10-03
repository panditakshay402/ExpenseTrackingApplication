namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class AssignTransactionCategoriesViewModel
{
    public int BudgetCategoryId { get; set; }
    public List<string> AllTransactionCategories { get; set; }
    public List<string> SelectedCategories { get; set; }
}

