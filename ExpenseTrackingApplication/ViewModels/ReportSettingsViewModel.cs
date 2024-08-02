using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class ReportSettingsViewModel
{
    [Required]
    public ReportType Type { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IncludeAllBudgets { get; set; }
    public List<int> SelectedBudgetIds { get; set; }
    public List<BudgetViewModel> AvailableBudgets { get; set; }
}
