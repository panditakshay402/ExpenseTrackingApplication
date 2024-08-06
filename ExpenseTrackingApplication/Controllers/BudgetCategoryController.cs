using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetCategoryController : Controller
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BudgetCategoryController(IBudgetCategoryRepository budgetCategoryRepository, IBudgetRepository budgetRepository, ITransactionRepository transactionRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        
    }
    
    // GET: BudgetCategory/AddNewBudgetCategory
    public async Task<IActionResult> AddNewBudgetCategory(int budgetId)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Invalid budget ID.");
            return RedirectToAction("Details", "Budget", new { id = budgetId });
        }

        var existingCategories = await _budgetCategoryRepository.GetByBudgetIdAsync(budgetId);
        int categoryCount = existingCategories.Count();

        var newCategory = new BudgetCategory
        {
            Name = $"Category {categoryCount + 1}",
            Type = BudgetCategoryType.Expenses,
            CurrentBalance = 0,
            Limit = 0,
            BudgetId = budgetId
        };

        if (await _budgetCategoryRepository.AddAsync(newCategory))
        {
            return RedirectToAction("Edit", new { id = newCategory.Id });
        }

        ModelState.AddModelError("", "Error while creating category.");
        return RedirectToAction("Details", "Budget", new { id = budgetId });
    }

    // GET: BudgetCategory/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }

        return View(budgetCategory);
    }

    // POST: BudgetCategory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetCategory budgetCategory)
    {
        if (id != budgetCategory.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var result = await _budgetCategoryRepository.UpdateAsync(budgetCategory);
                if (result)
                {
                    return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
                }
            }
            catch (Exception)
            {
                if (await _budgetCategoryRepository.GetByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ModelState.AddModelError("", "Error while updating category.");
        }

        return View(budgetCategory);
    }

    // POST: BudgetCategory/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int budgetId)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }

        var result = await _budgetCategoryRepository.DeleteAsync(budgetCategory);
        if (result)
        {
            return RedirectToAction("Details", "Budget", new { id = budgetId });
        }

        ModelState.AddModelError("", "Error while deleting category.");
        return RedirectToAction("Details", "Budget", new { id = budgetId });
    }
    
}