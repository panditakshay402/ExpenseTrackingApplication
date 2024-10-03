using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using ExpenseTrackingApplication.ViewModels.BudgetViewModels;
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

        var budgetCategory = new BudgetCategory
        {
            Name = $"Category {categoryCount + 1}",
            CurrentSpending = 0,
            Limit = 0,
            BudgetId = budgetId,
            BudgetCategoryTransactionCategories = new List<BudgetCategoryTransactionCategory>()

        };

        if (await _budgetCategoryRepository.AddAsync(budgetCategory))
        {
            return RedirectToAction("Edit", new { id = budgetCategory.Id });
        }

        ModelState.AddModelError("", "Error while creating category.");
        return RedirectToAction("Details", "Budget", new { id = budgetId });
    }

    // GET: BudgetCategory/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(budgetCategory.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        var viewModel = new BudgetCategoryEditViewModel
        {
            Id = budgetCategory.Id,
            Name = budgetCategory.Name,
            Limit = budgetCategory.Limit
        };

        return View(viewModel);
    }

    // POST: BudgetCategory/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetCategoryEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }
    
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to update the budget category. Please correct the errors and try again.");
            return View(viewModel);
        }

        // Update the category with the new values
        budgetCategory.Name = viewModel.Name;
        budgetCategory.Limit = viewModel.Limit;

        // Update the category in the database
        await _budgetCategoryRepository.UpdateAsync(budgetCategory);
        return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
    }
    
    // GET: BudgetCategory/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(budgetCategory.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        return View(budgetCategory);
    }
    
    // POST: BudgetCategory/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }

        var result = await _budgetCategoryRepository.DeleteAsync(budgetCategory);
        if (result)
        {
            return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
        }

        ModelState.AddModelError("", "Error while deleting category.");
        return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
    }
    
    // GET: BudgetCategory/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(id);
        if (budgetCategory == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(budgetCategory.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
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