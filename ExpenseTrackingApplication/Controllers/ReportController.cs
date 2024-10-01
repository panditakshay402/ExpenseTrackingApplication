﻿using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly IReportRepository _reportRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIncomeRepository _incomeRepository;

    public ReportController(IReportRepository reportRepository, UserManager<AppUser> userManager, IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, ITransactionRepository transactionRepository, IIncomeRepository incomeRepository)
    {
        _userManager = userManager;
        _budgetRepository = budgetRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
        _reportRepository = reportRepository;
        _transactionRepository = transactionRepository;
        _incomeRepository = incomeRepository;
        
    }
    
    public async Task<IActionResult> Index()
    {
        // Get the current user
        var user = await _userManager.GetUserAsync(User);

        // Retrieve budgets for the current user
        var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);
    
        // Retrieve reports for the budgets of the current user
        var reports = await _reportRepository.GetReportsByBudgetsAsync(budgets.Select(b => b.Id));

        // Map the retrieved reports to a view model
        var reportViewModels = reports.Select(report => new ReportViewModel
        {
            ReportId = report.Id,
            ReportName = report.ReportName,
            CreatedAt = report.CreatedDate,
            ReportType = report.Type.ToString(),
        }).ToList();

        // Return the view with the list of reports
        return View(reportViewModels);
    }
    
    // TODO: Koniecznie, pododawać modelstate.valid do wszystkich akcji, do trend analisys dodać incomes, potestować jeszcze i poprawić widoki,
    // ponieważ wywala jak coś źle się wpisze (np. miesiąc)
    
    // GET: Report/CreateMonthlySummary
    public async Task<IActionResult> CreateMonthlySummary()
    {
        var user = await _userManager.GetUserAsync(User);
        var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);

        var model = new CreateMonthlySummaryViewModel
        {
            AvailableBudgets = budgets.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList()
        };

        return View(model);
    }
    
    // POST: Report/CreateMonthlySummary
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMonthlySummary([Bind("BudgetId,Year,Month")] CreateMonthlySummaryViewModel model)
    {
        var budget = await _budgetRepository.GetByIdAsync(model.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Selected budget not found.");
            return View(model);
        }
        
        // Generate the monthly summary
        var summary = await GetMonthlySummaryAsync(model.BudgetId, model.Year, model.Month);
        
        // Create a new report
        var report = new Report
        {
            Type = ReportType.MonthlySummary,
            ReportName = $"Monthly Summary for {model.Month}/{model.Year}",
            CreatedDate = DateTime.Now,
            BudgetId = model.BudgetId,
            DateOne = new DateTime(model.Year, model.Month, 1),  // Ser DateOne as the month beginning
            DateTwo = new DateTime(model.Year, model.Month, 1).AddMonths(1).AddDays(-1) // Set DateTwo as the month end
        };
        await _reportRepository.AddAsync(report);
        
        // Redirect to the index or another action after creating the report
        return View("MonthlySummary", summary);
    }
    
    public async Task<MonthlySummaryViewModel> GetMonthlySummaryAsync(int budgetId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        var budgetName = budget.Name;
        
        // Get transactions and incomes for the month
        var transactions = (await _transactionRepository.GetByDateRangeAsync(budgetId, startDate, endDate)).ToList();
        var incomes = (await _incomeRepository.GetByDateRangeAsync(budgetId, startDate, endDate)).ToList();

        // Calculate total income, total expenses, and net savings
        var totalIncome = incomes.Sum(i => i.Amount);
        var totalExpenses = transactions.Sum(e => e.Amount);
        var netSavings = totalIncome - totalExpenses;

        return new MonthlySummaryViewModel
        {
            BudgetName = budgetName,
            Year = year,
            Month = month,
            TotalExpenses = totalExpenses,
            TotalIncome = totalIncome,
            NetSavings = netSavings,
            Transactions = transactions.ToList(),
            Incomes = incomes.ToList()
        };
    }
    
    // GET: Report/CreateMonthlySummary
    public async Task<IActionResult> CreateExpenseByCategory()
    {
        var user = await _userManager.GetUserAsync(User);
        var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);

        var model = new CreateExpenseByCategoryViewModel
        {
            AvailableBudgets = budgets.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList()
        };

        return View(model);
    }
    
    // POST: Report/CreateExpenseByCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExpenseByCategory([Bind("BudgetId,StartDate,EndDate")] CreateExpenseByCategoryViewModel model)
    {
        var budget = await _budgetRepository.GetByIdAsync(model.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Selected budget not found.");
            return View(model);
        }
            
        var expenseReport = await GetExpensesByCategoryAsync(model.BudgetId, model.StartDate, model.EndDate);
        return View("ExpenseByCategory", expenseReport);
    }
    
    public async Task<ExpenseByCategoryViewModel> GetExpensesByCategoryAsync(int budgetId, DateTime startDate, DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByDateRangeAsync(budgetId, startDate, endDate);

        var expensesByCategory = transactions
            .GroupBy(t => t.Category)
            .Select(group => new CategoryExpense
            {
                Category = group.Key,
                TotalAmount = group.Sum(t => t.Amount)
            })
            .ToList();

        return new ExpenseByCategoryViewModel
        {
            ExpensesByCategory = expensesByCategory
        };
    }

    // GET: Report/CreateTrendAnalysis
    public async Task<IActionResult> CreateTrendAnalysis()
    {
        var user = await _userManager.GetUserAsync(User);
        var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);

        var model = new CreateTrendAnalysisViewModel
        {
            AvailableBudgets = budgets.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList()
        };

        return View(model);
    }
    
    // POST: Report/CreateTrendAnalysis
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTrendAnalysis([Bind("BudgetId")] CreateTrendAnalysisViewModel model)
    {
        var trendData = await GetTrendAnalysisAsync(model.BudgetId);
        return View("TrendAnalysis", trendData);
    }
    
    public async Task<TrendAnalysisViewModel> GetTrendAnalysisAsync(int budgetId)
    {
        var transactions = await _transactionRepository.GetByBudgetAsync(budgetId);

        var monthlySpending = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(group => new MonthlySpending
            {
                Month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy"),
                TotalSpent = group.Sum(t => t.Amount)
            })
            .ToList();
    
        return new TrendAnalysisViewModel
        {
            MonthlySpendingData = monthlySpending
        };
    }


}