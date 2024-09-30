using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class CreateExpenseByCategoryViewModel
{
    public int BudgetId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // Start of current month
    public DateTime EndDate { get; set; } = DateTime.Today;
    public List<SelectListItem> AvailableBudgets { get; set; }
}