using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly IReportService _reportService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ReportController(IReportService reportService, UserManager<AppUser> userManager, ApplicationDbContext context)
    {
        _reportService = reportService;
        _userManager = userManager;
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var budgets = await _context.Budgets
            .Where(b => b.AppUserId == user.Id)
            .Select(b => new BudgetViewModel { Id = b.Id, Name = b.Name })
            .ToListAsync();

        var model = new ReportSettingsViewModel
        {
            AvailableBudgets = budgets
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateReport(ReportSettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var budgets = await _context.Budgets
                .Where(b => b.AppUserId == user.Id)
                .Select(b => new BudgetViewModel { Id = b.Id, Name = b.Name })
                .ToListAsync();

            model.AvailableBudgets = budgets;
            return View("Index", model);
        }

        var reportResult = await _reportService.GenerateReportAsync(model);

        // Return the file for download
        return File(reportResult.Content, reportResult.ContentType, reportResult.FileName);
    }

}