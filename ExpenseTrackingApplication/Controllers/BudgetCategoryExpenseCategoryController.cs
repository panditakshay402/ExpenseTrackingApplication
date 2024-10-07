using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.BudgetViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class BudgetCategoryExpenseCategoryController : Controller
{
    private readonly IBudgetCategoryExpenseCategoryRepository _bCtcRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly INotificationRepository _notificationRepository;

    public BudgetCategoryExpenseCategoryController(IBudgetCategoryExpenseCategoryRepository bCtcRepository, IBudgetCategoryRepository budgetCategoryRepository, IExpenseRepository expenseRepository, INotificationRepository notificationRepository, IBudgetRepository budgetRepository)
    {
        _bCtcRepository = bCtcRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
        _expenseRepository = expenseRepository;
        _notificationRepository = notificationRepository;
        _budgetRepository = budgetRepository;
    }
    
    // GET: BudgetCategoryExpenseCategory/AssignExpenseCategories
    public async Task<IActionResult> AssignExpenseCategories(int budgetCategoryId)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(budgetCategoryId);
        if (budgetCategory == null)
        {
            ModelState.AddModelError("", "Invalid budget category ID.");
            return RedirectToAction("Overview", "Budget");
        }
        
        var selectedCategories = await _bCtcRepository.GetExpenseCategoriesByBudgetCategoryIdAsync(budgetCategoryId);
        
        var viewModel = new AssignExpenseCategoriesViewModel
        {
            BudgetCategoryId = budgetCategoryId,
            AllExpenseCategories = Enum.GetNames(typeof(ExpenseCategory)).ToList(),
            SelectedCategories = selectedCategories.Select(c => c.ToString()).ToList()
        };

        return View(viewModel);
    }
    
    // POST: BudgetCategoryExpenseCategory/AssignExpenseCategories
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignExpenseCategories(AssignExpenseCategoriesViewModel viewModel)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(viewModel.BudgetCategoryId);
        if (budgetCategory == null)
        {
            ModelState.AddModelError("", "Invalid budget category ID.");
            return RedirectToAction("Overview", "Budget");
        }
        
        var budget = await _budgetRepository.GetByIdAsync(budgetCategory.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Invalid budget ID.");
            return RedirectToAction("Overview", "Budget");
        }
        
        var selectedCategories = viewModel.SelectedCategories ?? new List<string>();
        
        await _bCtcRepository.ClearByBudgetCategoryIdAsync(viewModel.BudgetCategoryId);

        foreach (var category in selectedCategories)
        {
            var bCtc = new BudgetCategoryExpenseCategory
            {
                BudgetCategoryId = viewModel.BudgetCategoryId,
                ExpenseCategory = category
            };
            await _bCtcRepository.AddAsync(bCtc);
        }
        
        var currentMonthSpending = await _expenseRepository.GetCurrentMonthAmountForCategoriesAsync(budgetCategory.BudgetId, selectedCategories.Select(Enum.Parse<ExpenseCategory>).ToList());
        
        // Update the current spending amount for the budget category
        budgetCategory.CurrentSpending = currentMonthSpending;
        await _budgetCategoryRepository.UpdateAsync(budgetCategory);
        
        if (budgetCategory.CurrentSpending > budgetCategory.Limit && budgetCategory.Limit > 0)
        {
            // Sent a notification to the user if the spending exceeds the limit
            await _notificationRepository.SendNotificationAsync(
                budget.AppUserId,
                "Budget Limit Exceeded",
                $"Your spending has exceeded the limit of {budgetCategory.Limit:C} for the category '{budgetCategory.Name}'.",
                NotificationType.Budget
            );
        }
        
        return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
    }
    
}