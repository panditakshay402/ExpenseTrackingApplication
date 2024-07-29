using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class IncomeController : Controller
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IBudgetRepository _budgetRepository;
    public IncomeController(IIncomeRepository incomeRepository, IBudgetRepository budgetRepository)
    {
        _incomeRepository = incomeRepository;
        _budgetRepository = budgetRepository;
    }
    
    // GET: Income/Create
    public IActionResult Create(int budgetId)
    {
        ViewBag.BudgetId = budgetId;
        return View();
    }
    
    // POST: Income/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Id,Source,Amount,Date,Category,Description")] Income income)
    {
        if (ModelState.IsValid)
        {
            income.BudgetId = budgetId;

            if (await _incomeRepository.AddAsync(income))
            {
                return RedirectToAction("Details", "Budget", new { id = budgetId });
            }
        }
        ViewBag.BudgetId = budgetId;
        return View(income);
    }
    
    // GET: Income/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var income = await _incomeRepository.GetByIdAsync(id.Value);
        if (income == null)
        {
            return NotFound();
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");

        var budget = await _budgetRepository.GetByIdAsync(income.BudgetId);
        if (budget == null || budget.AppUserId != userId)
        {
            return NotFound();
        }

        
        return View(income);
    }
    
    // GET: Income/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var income = await _incomeRepository.GetByIdAsync(id);
        if (income == null)
        {
            return NotFound();
        }
        
        var incomeViewModel = new EditIncomeViewModel
        {
            Id = income.Id,
            Source = income.Source,
            Amount = income.Amount,
            Date = income.Date,
            Category = income.Category,
            Description = income.Description,
            BudgetId = income.BudgetId
        };
        
        return View(incomeViewModel);
    }
    
    // POST: Income/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditIncomeViewModel incomeViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit income.");
            return View("Edit", incomeViewModel);
        }

        var income = await _incomeRepository.GetByIdAsync(id);
        if (income == null)
        {
            return NotFound();
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");
        var budget = await _budgetRepository.GetByIdAsync(income.BudgetId);
        if (budget == null || budget.AppUserId != userId)
        {
            return NotFound();
        }
    
        income.Source = incomeViewModel.Source;
        income.Amount = incomeViewModel.Amount;
        income.Date = incomeViewModel.Date;
        income.Category = incomeViewModel.Category;
        income.Description = incomeViewModel.Description;

        await _incomeRepository.UpdateAsync(income);

        return RedirectToAction("Details", "Budget", new { id = incomeViewModel.BudgetId });
    }
    
    // GET: Income/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var incomeDetails = await _incomeRepository.GetByIdAsync(id);
        if (incomeDetails == null)
        {
            return NotFound();
        }
        
        return View(incomeDetails);
    }
    
    // POST: Income/Delete/{id}
    [HttpPost, ActionName("DeleteIncome")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var income = await _incomeRepository.GetByIdAsync(id);
        if (income == null)
        {
            return NotFound();
        }
        
        int budgetId = income.BudgetId;
        if (await _incomeRepository.DeleteAsync(income))
        {
            return RedirectToAction("Details", "Budget", new { id = budgetId });
        }
        
        return RedirectToAction("Error", "Home");
    }
    
}