using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        
        if (userId == null)
        {
            return NotFound();
        }
        
        // Get all budgets that belong to the currently logged-in user
        var budgets = await _budgetRepository.GetBudgetByUserAsync(userId);

        return View(budgets);
    }
    
    // GET: Budget/AddNewBudget
    public async Task<IActionResult> AddNewBudget()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null)
        {
            return NotFound();
        }

        // Get the current number of budgets
        var budgets = await _budgetRepository.GetBudgetByUserAsync(userId);
        int newBudgetNumber = budgets.Count() + 1; // Calculate next number
        
        var newBudgetName = $"Budget {newBudgetNumber}"; // Generate name

        var newBudget = new Budget
        {
            Name = newBudgetName,
            Balance = 0,
            AppUserId = userId
        };

        if (await _budgetRepository.AddAsync(newBudget))
        {
            await _budgetRepository.SaveAsync(); // Ensure changes are saved
            return RedirectToAction(nameof(Index));
        }

        return View("Index"); // Redirect to Index in case of failure
    }
    
    // GET: Budget/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Budget/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Amount")] Budget budget)
    {
        if (ModelState.IsValid)
        {
            budget.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                ?? throw new ArgumentNullException(nameof(User), "User identifier not found");

            if (await _budgetRepository.AddAsync(budget))
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(budget);
    }
    
    // GET: Budget/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        var budgetViewModel = new EditBudgetViewModel
        {
            Id = budget.Id,
            Balance = budget.Balance,
            AppUserId = budget.AppUserId
        };
        return View(budgetViewModel); 
    }

    // POST: Budget/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditBudgetViewModel budgetViewModel)
    {
        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit budget.");
            return View("Edit", budgetViewModel);
        }
    
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        budget.Balance = budgetViewModel.Balance;
    
        await _budgetRepository.UpdateAsync(budget);
    
        return RedirectToAction("Index");
    }
    
    // GET: Budget/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var budgetDetails = await _budgetRepository.GetByIdAsync(id);
        if (budgetDetails == null)
        {
            return NotFound();
        }
    
        return View(budgetDetails);
    }
    
    // POST: Budget/Delete/{id}
    [HttpPost, ActionName("DeleteBudget")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        await _budgetRepository.DeleteAsync(budget);
        return RedirectToAction("Index");
    }
}