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
    private readonly INotificationService _notificationService;
    public IncomeController(IIncomeRepository incomeRepository, IBudgetRepository budgetRepository, INotificationService notificationService)
    {
        _incomeRepository = incomeRepository;
        _budgetRepository = budgetRepository;
        _notificationService = notificationService;
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
                var budget = await _budgetRepository.GetByIdAsync(budgetId);
                if (budget != null)
                {
                    budget.Balance += income.Amount;
                    await _budgetRepository.UpdateAsync(budget);
                }
                
                return RedirectToAction("Edit", "Budget", new { id = budgetId });
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
        
        var incomeViewModel = new IncomeEditViewModel
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
    public async Task<IActionResult> Edit(int id, IncomeEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit income.");
            return View("Edit", viewModel);
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
        
        // Calculate new balance
        var previousAmount = income.Amount;
        var newAmount = viewModel.Amount;
        
        // Update income details
        income.Source = viewModel.Source;
        income.Amount = viewModel.Amount;
        income.Date = viewModel.Date;
        income.Category = viewModel.Category;
        income.Description = viewModel.Description;
        
        // Update the budget balance
        budget.Balance -= previousAmount - newAmount;
        
        // Update the repositories
        await _incomeRepository.UpdateAsync(income);
        await _budgetRepository.UpdateAsync(budget);
        
        return RedirectToAction("Edit", "Budget", new { id = income.BudgetId });
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
        
        // Get the budget associated with the income
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Update the budget balance
        budget.Balance -= income.Amount;
        await _budgetRepository.UpdateAsync(budget);
        
        if (await _incomeRepository.DeleteAsync(income))
        {
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }
        
        return RedirectToAction("Error", "Home");
    }
    
}