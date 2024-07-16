using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetController : Controller
{
    private readonly ApplicationDbContext _context;
    public BudgetController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Budget
    public async Task<IActionResult> Index()
    {
        // Get the ID of the currently logged-in user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Get all budgets that belong to the currently logged-in user
        var budgets = await _context.Budgets
            .Where(b => b.AppUserId == userId)
            .ToListAsync();

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

            _context.Add(budget);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(budget);
    }
}