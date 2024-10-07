using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels.ReportViewModels;

public class ExpensesByCategoryViewModel
{
    public List<CategoryExpenses> ExpensesByCategory { get; set; } = new List<CategoryExpenses>();
}

public class CategoryExpenses
{
    public ExpenseCategory Category { get; set; }
    public decimal TotalAmount { get; set; }
}
