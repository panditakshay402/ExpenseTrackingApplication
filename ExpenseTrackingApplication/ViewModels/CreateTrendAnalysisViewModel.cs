using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class CreateTrendAnalysisViewModel
{
    public int BudgetId { get; set; }
    public List<SelectListItem> AvailableBudgets { get; set; }
}