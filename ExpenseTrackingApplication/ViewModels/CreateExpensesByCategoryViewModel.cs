using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class CreateExpensesByCategoryViewModel
{
    public int BudgetId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
    public DateTime EndDate { get; set; } = DateTime.Today;
    public List<SelectListItem> AvailableBudgets { get; set; }
}