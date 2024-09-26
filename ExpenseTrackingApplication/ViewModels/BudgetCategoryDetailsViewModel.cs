using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Limit { get; set; }
}

