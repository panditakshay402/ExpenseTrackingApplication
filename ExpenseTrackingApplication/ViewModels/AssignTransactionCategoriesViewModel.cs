using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class AssignTransactionCategoriesViewModel
{
    public int BudgetCategoryId { get; set; }
    public List<TransactionCategory> AllTransactionCategories { get; set; }
    public List<TransactionCategory> SelectedCategories { get; set; }
}
