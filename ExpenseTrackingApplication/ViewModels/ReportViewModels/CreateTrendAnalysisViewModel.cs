using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels.ReportViewModels;

public class CreateTrendAnalysisViewModel
{
    public int BudgetId { get; set; }
    public List<SelectListItem> AvailableBudgets { get; set; }
}