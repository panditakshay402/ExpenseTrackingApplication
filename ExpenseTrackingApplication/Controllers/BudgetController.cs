using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetController : Controller
{
    private readonly IBudgetRepository _budgetRepository;
    public BudgetController(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }
    
    // GET: Budget
    public async Task<IActionResult> Index()
    {
        // Get the ID of the currently logged-in user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Get all budgets that belong to the currently logged-in user
        var budgets = await _budgetRepository.GetBudgetByUser(userId);

        return View(budgets);
    }
    
    // GET: Budget/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Budget/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Amount,Limit,AppUserId")] Budget budget)
    {
        if (ModelState.IsValid)
        {
            // Get the ID of the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
            // Set the AppUserId of the budget to the ID of the currently logged-in user
            budget.AppUserId = userId;

            if (await _budgetRepository.AddAsync(budget))
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(budget);
    }
}