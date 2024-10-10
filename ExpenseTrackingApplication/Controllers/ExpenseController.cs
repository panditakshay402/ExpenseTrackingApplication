using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.TransactionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class ExpenseController : Controller
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetCategoryExpenseCategoryRepository _bCtcRepository;
    private readonly INotificationRepository _notificationRepository;
    public ExpenseController(IExpenseRepository expenseRepository, IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, INotificationRepository notificationRepository, IBudgetCategoryExpenseCategoryRepository bCtcRepository)
    {
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _notificationRepository = notificationRepository;
        _bCtcRepository = bCtcRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
    }
    
    // GET: Expense/Create
    public async Task<IActionResult> Create(int budgetId)
    {
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(budgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        ViewBag.BudgetId = budgetId;
        return PartialView("_CreateExpensePartialView", new Expense { BudgetId = budgetId });
    }
    
    // POST: Expense/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Recipient,Amount,Date,Category,Description")] Expense expense)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BudgetId = budgetId;
            return PartialView("_CreateExpensePartialView", expense); // Return the view with the error messages
        }
    
        expense.BudgetId = budgetId;
        var category = expense.Category;

        if (await _expenseRepository.AddAsync(expense))
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget != null)
            {
                budget.Balance -= expense.Amount;
                await _budgetRepository.UpdateAsync(budget);
                await UpdateBcSpending(budgetId, expense.Date, category);
            }
        
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }

        // If something went wrong
        ViewBag.BudgetId = budgetId;
        return PartialView("_CreateExpensePartialView", expense);
    }
    
    // GET: Expense/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var expense = await _expenseRepository.GetByIdAsync(id.Value);
        if (expense == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(expense.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }

        return View(expense);
    }
    
    // GET: Expense/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(expense.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        var expenseEditViewModel = new ExpenseEditViewModel
        {
            Id = expense.Id,
            Recipient = expense.Recipient,
            Amount = expense.Amount,
            Date = expense.Date,
            Category = expense.Category,
            Description = expense.Description,
            BudgetId = expense.BudgetId
        };

        return View(expenseEditViewModel);
    }
    
    // POST: Expense/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit expense.");
            return View("Edit", viewModel);
        }

        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        
        var budget = await _budgetRepository.GetByIdAsync(expense.BudgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Calculate new balance
        var previousAmount = expense.Amount;
        var newAmount = viewModel.Amount;
        
        // Update expense details
        expense.Recipient = viewModel.Recipient;
        expense.Amount = viewModel.Amount;
        expense.Date = viewModel.Date;
        expense.Category = viewModel.Category;
        expense.Description = viewModel.Description;

        // Update the budget balance
        budget.Balance += previousAmount - newAmount;
    
        // Update the repositories
        await _expenseRepository.UpdateAsync(expense);
        await _budgetRepository.UpdateAsync(budget);
        
        await UpdateBcSpending(budget.Id, expense.Date, expense.Category);
        
        return RedirectToAction("Edit", "Budget", new { id = expense.BudgetId });
    }
    
    // GET: Expense/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(expense.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        return View(expense);
    }
    
    // POST: Expense/Delete/{id}
    [HttpPost, ActionName("DeleteExpense")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        var budgetId = expense.BudgetId;

        // Get the budget associated with the expense
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Update the budget balance
        budget.Balance += expense.Amount;
        await _budgetRepository.UpdateAsync(budget);
        
        var date = expense.Date;
        var category = expense.Category;
        
        // Delete the expense
        if (await _expenseRepository.DeleteAsync(expense))
        {
            await UpdateBcSpending(budgetId, date, category);
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }

        return RedirectToAction("Error", "Home");
    }
    
    private async Task UpdateBcSpending(int budgetId, DateTime expenseDate, ExpenseCategory category)
    {
        // Get all budget categories associated with the budget
        var budgetCategories = await _budgetCategoryRepository.GetByBudgetIdAsync(budgetId);
        
        // Set the start and end date of the current month
        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        // Check if the expense date is within the current month
        if (expenseDate < startDate || expenseDate > endDate)
        {
            // If the expense date is not within the current month, return
            return;
        }
        
        foreach (var budgetCategory in budgetCategories)
        {
            // Get the expense categories associated with the budget category
            var expenseCategories = await _bCtcRepository.GetExpenseCategoriesByBudgetCategoryIdAsync(budgetCategory.Id);
            
            // Check if the expense category is associated with the budget category
            if (expenseCategories.Contains(category))
            {
                // Get the current month spending for the budget category
                var currentMonthSpending =
                    await _expenseRepository.GetCurrentMonthAmountForCategoriesAsync(budgetId,
                        expenseCategories);

                // Update the current spending for the budget category
                budgetCategory.CurrentSpending = currentMonthSpending;
                await _budgetCategoryRepository.UpdateAsync(budgetCategory);

                // Check if the current spending exceeds the limit
                if (budgetCategory.CurrentSpending > budgetCategory.Limit)
                {
                    var budget = await _budgetRepository.GetByIdAsync(budgetId);
                    if (budget != null)
                    {
                        await _notificationRepository.SendNotificationAsync(
                            budget.AppUserId,
                            "Budget Limit Exceeded",
                            $"Your spending has exceeded the limit of {budgetCategory.Limit:C} for the category '{budgetCategory.Name}'.",
                            NotificationType.Budget
                        );
                    }
                }
            }
        }
        
    }
    
    // Check if the user owns the budget
    private async Task<IActionResult?> CheckUserOwnership(int budgetId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return NotFound(); // Return 404 if user is not found
        }
        
        if (!await _budgetRepository.UserOwnsBudgetAsync(budgetId, userId))
        {
            return NotFound(); // Return 404 if the user does not own the budget
        }

        return null; // Return null if the user owns the budget
    }
}