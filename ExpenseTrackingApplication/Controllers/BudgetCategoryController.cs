using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetCategoryController : Controller
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetCategoryTransactionCategoryRepository _bCtcRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly INotificationRepository _notificationRepository;

    public BudgetCategoryController(IBudgetCategoryRepository budgetCategoryRepository,
        IBudgetRepository budgetRepository, ITransactionRepository transactionRepository,
        IIncomeRepository incomeRepository, INotificationRepository notificationRepository,
        IBudgetCategoryTransactionCategoryRepository bCtcRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
        _bCtcRepository = bCtcRepository;
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        _incomeRepository = incomeRepository;
        _notificationRepository = notificationRepository;
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
            CurrentSpending = 0,
            Limit = 0,
            BudgetId = budgetId,
            BudgetCategoryTransactionCategories = new List<BudgetCategoryTransactionCategory>()

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
        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        
        var viewModel = new BudgetCategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Limit = category.Limit
        };

        return View(viewModel);
    }

    // POST: BudgetCategory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetCategoryEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
    
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to update the budget category. Please correct the errors and try again.");
            return View(viewModel);
        }

        // Update the category with the new values
        category.Name = viewModel.Name;
        category.Limit = viewModel.Limit;

        // Update the category in the database
        await _budgetCategoryRepository.UpdateAsync(category);
        return RedirectToAction("Details", "Budget", new { id = category.BudgetId });
    }
    
    // GET: BudgetCategory/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }
    
    // POST: BudgetCategory/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var result = await _budgetCategoryRepository.DeleteAsync(category);
        if (result)
        {
            return RedirectToAction("Details", "Budget", new { id = category.BudgetId });
        }

        ModelState.AddModelError("", "Error while deleting category.");
        return RedirectToAction("Details", "Budget", new { id = category.BudgetId });
    }
    
    // GET: BudgetCategory/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }
        
        var categories = await _bCtcRepository.GetTransactionCategoriesByBudgetCategoryIdAsync(id);
        var transactions = await _transactionRepository.GetTransactionsByCategoriesAsync(budgetCategory.BudgetId, categories);
        
        var viewModel = new BudgetCategoryDetailsViewModel
        {
            BudgetCategory = budgetCategory,
            CategoryTransactions = transactions
        };
        
        return View(viewModel);
    }



}