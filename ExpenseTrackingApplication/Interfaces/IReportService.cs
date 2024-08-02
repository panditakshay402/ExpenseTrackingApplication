using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;

namespace ExpenseTrackingApplication.Interfaces;

public interface IReportService
{
    Task<ReportResult> GenerateReportAsync(ReportSettingsViewModel settings);
}