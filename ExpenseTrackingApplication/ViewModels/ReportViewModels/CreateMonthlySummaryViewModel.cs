using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels.ReportViewModels;

public class CreateMonthlySummaryViewModel
{
    public int BudgetId { get; set; }
    public string BudgetName { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public int Month { get; set; } = DateTime.Today.Month;
    public List<SelectListItem> AvailableBudgets { get; set; }
}
