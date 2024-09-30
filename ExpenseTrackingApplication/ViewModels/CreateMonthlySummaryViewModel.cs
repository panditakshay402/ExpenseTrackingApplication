using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class CreateMonthlySummaryViewModel
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<SelectListItem> AvailableBudgets { get; set; }
}
