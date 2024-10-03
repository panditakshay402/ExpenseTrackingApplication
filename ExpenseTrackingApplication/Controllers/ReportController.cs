using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
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
    public async Task<IActionResult> CreateExpensesByCategory()
    {
        var user = await _userManager.GetUserAsync(User);
        var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);

        var model = new CreateExpensesByCategoryViewModel
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
    public async Task<IActionResult> CreateExpensesByCategory([Bind("BudgetId,StartDate,EndDate")] CreateExpensesByCategoryViewModel model)
    {
        var budget = await _budgetRepository.GetByIdAsync(model.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Selected budget not found.");
            return View(model);
        }
        
        // Dates validation
        if (model.StartDate.Year < 2000 || model.EndDate.Year < 2000)
        {
            ModelState.AddModelError("", "Dates must be reasonable (after year 2000).");
        }
        if (model.StartDate > model.EndDate)
        {
            ModelState.AddModelError("", "Start date must be earlier than end date.");
        }

        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var budgets = await _budgetRepository.GetBudgetByUserAsync(user.Id);

            model.AvailableBudgets = budgets.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            return View(model);
        }
            
        var expenseReport = await GetExpensesByCategoryAsync(model.BudgetId, model.StartDate, model.EndDate);
        
        // Create a new report
        var report = new Report
        {
            Type = ReportType.ExpensesByCategory,
            ReportName = $"Expenses By Category for {budget.Name}",
            CreatedDate = DateTime.Now,
            BudgetId = model.BudgetId,
            DateOne = model.StartDate,
            DateTwo = model.EndDate
        };
        await _reportRepository.AddAsync(report);
        
        return View("ExpensesByCategory", expenseReport);
    }
    
    public async Task<ExpensesByCategoryViewModel> GetExpensesByCategoryAsync(int budgetId, DateTime startDate, DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByDateRangeAsync(budgetId, startDate, endDate);

        var expensesByCategory = transactions
            .GroupBy(t => t.Category)
            .Select(group => new CategoryExpenses
            {
                Category = group.Key,
                TotalAmount = group.Sum(t => t.Amount)
            })
            .ToList();

        return new ExpensesByCategoryViewModel
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
        var budget = await _budgetRepository.GetByIdAsync(model.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Selected budget not found.");
            return View(model);
        }
        
        // Create a new report
        var report = new Report
        {
            Type = ReportType.TrendAnalysis,
            ReportName = $"Trend Analysis for {budget.Name}",
            CreatedDate = DateTime.Now,
            BudgetId = model.BudgetId,
        };
        await _reportRepository.AddAsync(report);
        
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
    
    public async Task<IActionResult> ViewReport(int id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(report.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        // Check report type and return the appropriate view
        switch (report.Type)
        {
            case ReportType.MonthlySummary:
                var monthlySummary = await GetMonthlySummaryAsync(report.BudgetId, report.DateOne.Value.Year, report.DateOne.Value.Month);
                return View("MonthlySummary", monthlySummary);
        
            case ReportType.ExpensesByCategory:
                var expenseReport = await GetExpensesByCategoryAsync(report.BudgetId, report.DateOne.Value, report.DateTwo.Value);
                return View("ExpensesByCategory", expenseReport);
        
            case ReportType.TrendAnalysis:
                var trendAnalysis = await GetTrendAnalysisAsync(report.BudgetId);
                return View("TrendAnalysis", trendAnalysis);
        
            default:
                return BadRequest("Unknown report type.");
        }
    }
    
    // GET: Report/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(report.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        return View(report);
    }
    
    // POST: Report/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }
        
        var result = await _reportRepository.DeleteAsync(report);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }
        
        ModelState.AddModelError("", "Error while deleting category.");
        return RedirectToAction(nameof(Index));
    }
    
    // Check if the user owns the budget
    private async Task<IActionResult?> CheckUserOwnership(int budgetId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return NotFound(); // Return 404 if user is not logged in
        }
        
        if (!await _budgetRepository.UserOwnsBudgetAsync(budgetId, userId))
        {
            return NotFound(); // Return 404 if the user does not own the budget
        }

        return null; // Return null if the user owns the budget
    }
}