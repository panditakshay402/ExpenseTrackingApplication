using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.Repositories;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class BudgetCategoryTransactionCategoryController : Controller
{
    private readonly IBudgetCategoryTransactionCategoryRepository _bCtcRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;

    public BudgetCategoryTransactionCategoryController(IBudgetCategoryTransactionCategoryRepository bCtcRepository, IBudgetCategoryRepository budgetCategoryRepository, ITransactionRepository transactionRepository, INotificationRepository notificationRepository, IBudgetRepository budgetRepository)
    {
        _bCtcRepository = bCtcRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
        _budgetRepository = budgetRepository;
    }
    
    // GET: BudgetCategoryTransactionCategory/AssignTransactionCategories
    public async Task<IActionResult> AssignTransactionCategories(int budgetCategoryId)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(budgetCategoryId);
        if (budgetCategory == null)
        {
            ModelState.AddModelError("", "Invalid budget category ID.");
            return RedirectToAction("Index", "Budget");
        }
        
        var selectedCategories = await _bCtcRepository.GetTransactionCategoriesByBudgetCategoryIdAsync(budgetCategoryId);
        
        var viewModel = new AssignTransactionCategoriesViewModel
        {
            BudgetCategoryId = budgetCategoryId,
            AllTransactionCategories = Enum.GetNames(typeof(TransactionCategory)).ToList(),
            SelectedCategories = selectedCategories.Select(c => c.ToString()).ToList()
        };

        return View(viewModel);
    }
    
    // POST: BudgetCategoryTransactionCategory/AssignTransactionCategories
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignTransactionCategories(AssignTransactionCategoriesViewModel viewModel)
    {
        var budgetCategory = await _budgetCategoryRepository.GetByIdAsync(viewModel.BudgetCategoryId);
        if (budgetCategory == null)
        {
            ModelState.AddModelError("", "Invalid budget category ID.");
            return RedirectToAction("Index", "Budget");
        }
        
        var budget = await _budgetRepository.GetByIdAsync(budgetCategory.BudgetId);
        if (budget == null)
        {
            ModelState.AddModelError("", "Invalid budget ID.");
            return RedirectToAction("Index", "Budget");
        }
        
        var selectedCategories = viewModel.SelectedCategories ?? new List<string>();
        
        await _bCtcRepository.ClearByBudgetCategoryIdAsync(viewModel.BudgetCategoryId);

        foreach (var category in selectedCategories)
        {
            var bCtc = new BudgetCategoryTransactionCategory
            {
                BudgetCategoryId = viewModel.BudgetCategoryId,
                TransactionCategory = category
            };
            await _bCtcRepository.AddAsync(bCtc);
        }
        
        var currentMonthSpending = await _transactionRepository.GetCurrentMonthAmountForCategoriesAsync(budgetCategory.BudgetId, selectedCategories.Select(Enum.Parse<TransactionCategory>).ToList());
        
        // Update the current spending amount for the budget category
        budgetCategory.CurrentSpending = currentMonthSpending;
        await _budgetCategoryRepository.UpdateAsync(budgetCategory);
        
        if (budgetCategory.CurrentSpending > budgetCategory.Limit)
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