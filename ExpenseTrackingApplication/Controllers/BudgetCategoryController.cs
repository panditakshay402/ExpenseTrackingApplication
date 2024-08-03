using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class BudgetCategoryController : Controller
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;

    public BudgetCategoryController(IBudgetCategoryRepository budgetCategoryRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
    }

    // GET: BudgetCategory/Create
    public IActionResult Create(int budgetId)
    {
        ViewBag.BudgetId = budgetId;
        return View();
    }

    // POST: BudgetCategory/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BudgetCategory budgetCategory)
    {
        if (ModelState.IsValid)
        {
            budgetCategory.CreatedAt = DateTime.UtcNow; // Set creation date
            var result = await _budgetCategoryRepository.AddAsync(budgetCategory);
            if (result)
            {
                return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
            }

            ModelState.AddModelError("", "Error while creating category.");
        }

        return View(budgetCategory);
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