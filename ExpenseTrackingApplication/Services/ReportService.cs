using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.ViewModels;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReportResult> GenerateReportAsync(ReportSettingsViewModel settings)
    {
        var transactions = new List<Transaction>();

        if (settings.IncludeAllBudgets)
        {
            transactions = await _context.Transactions
                .Where(t => t.Date >= settings.StartDate && t.Date <= settings.EndDate)
                .ToListAsync();
        }
        else if (settings.SelectedBudgetIds.Any())
        {
            transactions = await _context.Transactions
                .Where(t => settings.SelectedBudgetIds.Contains(t.BudgetId) && t.Date >= settings.StartDate && t.Date <= settings.EndDate)
                .ToListAsync();
        }

        byte[] reportContent;

        switch (settings.Type)
        {
            case ReportType.MonthlySummary:
                reportContent = GenerateMonthlySummaryReport(settings, transactions);
                break;
            case ReportType.ExpensesByCategory:
                reportContent = GenerateExpensesByCategoryReport(settings, transactions);
                break;
            case ReportType.TrendAnalysis:
                reportContent = GenerateTrendAnalysisReport(settings, transactions);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new ReportResult
        {
            Content = reportContent,
            ContentType = "application/pdf",
            FileName = "report.pdf"
        };
    }

    private byte[] GenerateMonthlySummaryReport(ReportSettingsViewModel settings, List<Transaction> transactions)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 20, XFontStyle.Bold);
        gfx.DrawString("Monthly Summary Report", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

        // Add monthly summary details to the PDF report

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    private byte[] GenerateExpensesByCategoryReport(ReportSettingsViewModel settings, List<Transaction> transactions)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 20, XFontStyle.Bold);
        gfx.DrawString("Expenses By Category Report", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

        // Add expenses by category details to the PDF report

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    private byte[] GenerateTrendAnalysisReport(ReportSettingsViewModel settings, List<Transaction> transactions)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 20, XFontStyle.Bold);
        gfx.DrawString("Trend Analysis Report", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

        // Add trend analysis details to the PDF report

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}