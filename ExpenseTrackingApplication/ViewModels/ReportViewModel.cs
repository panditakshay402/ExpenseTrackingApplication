using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels;

public class ReportViewModel
{
    public int ReportId { get; set; }
    public string? ReportName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ReportType { get; set; }
    public string Data { get; set; }
}
