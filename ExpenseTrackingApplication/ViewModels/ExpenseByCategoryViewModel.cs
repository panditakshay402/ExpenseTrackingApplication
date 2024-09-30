using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class ExpenseByCategoryViewModel
{
    public List<CategoryExpense> ExpensesByCategory { get; set; } = new List<CategoryExpense>();
}

public class CategoryExpense
{
    public TransactionCategory Category { get; set; }
    public decimal TotalAmount { get; set; }
}
