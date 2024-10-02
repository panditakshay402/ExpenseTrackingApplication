using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class ExpensesByCategoryViewModel
{
    public List<CategoryExpenses> ExpensesByCategory { get; set; } = new List<CategoryExpenses>();
}

public class CategoryExpenses
{
    public TransactionCategory Category { get; set; }
    public decimal TotalAmount { get; set; }
}
