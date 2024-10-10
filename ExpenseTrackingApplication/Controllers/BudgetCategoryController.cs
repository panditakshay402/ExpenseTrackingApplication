using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.BudgetViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetCategoryController : Controller
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetCategoryExpenseCategoryRepository _bCtcRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly INotificationRepository _notificationRepository;

    public BudgetCategoryController(IBudgetCategoryRepository budgetCategoryRepository,
        IBudgetRepository budgetRepository, IExpenseRepository expenseRepository,
        IIncomeRepository incomeRepository, INotificationRepository notificationRepository,
        IBudgetCategoryExpenseCategoryRepository bCtcRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
        _bCtcRepository = bCtcRepository;
        _budgetRepository = budgetRepository;
        _expenseRepository = expenseRepository;
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
            BudgetCategoryExpenseCategories = new List<BudgetCategoryExpenseCategory>()

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
        
        return RedirectToAction("AssignExpenseCategories", "BudgetCategoryExpenseCategory", new { budgetCategoryId = viewModel.Id });


    }
    
    // POST: BudgetCategory/Delete/{id}
    [HttpPost, ActionName("DeleteBudgetCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
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
        
        // Delete the category
        if (await _budgetCategoryRepository.DeleteAsync(budgetCategory))
        {
            return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
        }
        
        return RedirectToAction("Error", "Home");
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
        
        var categories = await _bCtcRepository.GetExpenseCategoriesByBudgetCategoryIdAsync(id);
        var expenses = await _expenseRepository.GetExpensesByCategoriesAsync(budgetCategory.BudgetId, categories);
        
        var viewModel = new BudgetCategoryDetailsViewModel
        {
            BudgetCategory = budgetCategory,
            CategoryExpenses = expenses
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