namespace ExpenseTrackingApplication.ViewModels.ReportViewModels;

public class TrendAnalysisViewModel
{
    public List<MonthlySpending> MonthlySpendingData { get; set; } = new List<MonthlySpending>();
}

public class MonthlySpending
{
    public string Month { get; set; }
    public decimal TotalSpent { get; set; }
}
