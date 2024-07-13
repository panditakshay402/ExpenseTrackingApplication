using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;
    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Report
    public IActionResult Index()
    {
        return View();
    }
    
    // POST: Report/Generate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(DateTime startDate, DateTime endDate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        // Get transactions for the specified date range and user
        var transactions = await _context.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate && t.AppUserId == userId)
            .ToListAsync();

        // Create a new report
        var report = new Report
        {
            Type = "Custom",
            StartDate = startDate,
            EndDate = endDate,
            AppUserId = userId,
            Transactions = transactions  // Add transactions to the report
        };

        // Add the report to the database
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        // Redirect to the Report action with the report ID
        return RedirectToAction("Report", new { id = report.Id });
    }
    
    // GET: Report/Report/{id}
    public async Task<IActionResult> Report(int id)
    {
        var report = await _context.Reports
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
        {
            return NotFound();
        }

        return View(report);
    }
}