﻿using ExpenseTrackingApplication.Data;
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
    private readonly IReportService _reportService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIncomeRepository _incomeRepository;

    public ReportController(IReportService reportService, UserManager<AppUser> userManager, ITransactionRepository transactionRepository, IBudgetRepository budgetRepository, IIncomeRepository incomeRepository)
    {
        _reportService = reportService;
        _userManager = userManager;
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _incomeRepository = incomeRepository;
    }
    
    public IActionResult Index()
    {
        // Mock list of reports, replace with repository logic to fetch real reports.
        var reports = new List<ReportViewModel>
        {
            
        };
    
        return View(reports);
    }

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

        var summary = await GetMonthlySummaryAsync(budget.Id, model.Year, model.Month);
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
    
    // public async Task<ExpenseByCategoryViewModel> GetExpensesByCategoryAsync(int budgetId, DateTime startDate, DateTime endDate)
    // {
    //     var transactions = await _transactionRepository.GetByDateRangeAsync(budgetId, startDate, endDate);
    //         
    //     var expensesByCategory = transactions
    //         .Where(t => t.Type == TransactionType.Expense)
    //         .GroupBy(t => t.Category)
    //         .Select(group => new CategoryExpense
    //         {
    //             Category = group.Key,
    //             TotalAmount = group.Sum(t => t.Amount)
    //         })
    //         .ToList();
    //         
    //     return new ExpenseByCategoryViewModel
    //     {
    //         ExpensesByCategory = expensesByCategory
    //     };
    // }
    //
    // public async Task<List<MonthlyTrendViewModel>> GetSpendingTrendsAsync(int budgetId, DateTime startDate, DateTime endDate)
    // {
    //     var trends = new List<MonthlyTrendViewModel>();
    //
    //     for (var date = startDate; date <= endDate; date = date.AddMonths(1))
    //     {
    //         var summary = await GetMonthlySummaryAsync(budgetId, date.Year, date.Month);
    //         trends.Add(new MonthlyTrendViewModel
    //         {
    //             Month = date.ToString("MMM yyyy"),
    //             TotalExpenses = summary.TotalExpenses,
    //             TotalIncome = summary.TotalIncome
    //         });
    //     }
    //
    //     return trends;
    // }


}