using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class TransactionController : Controller
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetCategoryTransactionCategoryRepository _bCtcRepository;
    private readonly INotificationRepository _notificationRepository;
    public TransactionController(ITransactionRepository transactionRepository, IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, INotificationRepository notificationRepository, IBudgetCategoryTransactionCategoryRepository bCtcRepository)
    {
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _notificationRepository = notificationRepository;
        _bCtcRepository = bCtcRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
    }
    
    // GET: Transaction/Create
    public async Task<IActionResult> Create(int budgetId)
    {
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(budgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        ViewBag.BudgetId = budgetId;
        return View();
    }
    
    // POST: Transaction/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Recipient,Amount,Date,Category,Description")] Transaction transaction)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BudgetId = budgetId;
            return View(transaction); // Return the view with the error messages
        }
    
        transaction.BudgetId = budgetId;
        var category = transaction.Category;

        if (await _transactionRepository.AddAsync(transaction))
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget != null)
            {
                budget.Balance -= transaction.Amount;
                await _budgetRepository.UpdateAsync(budget);
                await UpdateBcSpending(budgetId, transaction.Date, category);
            }
        
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }

        // If something went wrong
        return RedirectToAction("Details", "Budget", new { id = budgetId });
    }
    
    // GET: Transaction/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaction = await _transactionRepository.GetByIdAsync(id.Value);
        if (transaction == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(transaction.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }

        return View(transaction);
    }
    
    // GET: Transaction/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(transaction.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        var transactionViewModel = new TransactionEditViewModel
        {
            Id = transaction.Id,
            Recipient = transaction.Recipient,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Category = transaction.Category,
            Description = transaction.Description,
            BudgetId = transaction.BudgetId
        };

        return View(transactionViewModel);
    }
    
    // POST: Transaction/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TransactionEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit transaction.");
            return View("Edit", viewModel);
        }

        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        
        var budget = await _budgetRepository.GetByIdAsync(transaction.BudgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Calculate new balance
        var previousAmount = transaction.Amount;
        var newAmount = viewModel.Amount;
        
        // Update transaction details
        transaction.Recipient = viewModel.Recipient;
        transaction.Amount = viewModel.Amount;
        transaction.Date = viewModel.Date;
        transaction.Category = viewModel.Category;
        transaction.Description = viewModel.Description;

        // Update the budget balance
        budget.Balance += previousAmount - newAmount;
    
        // Update the repositories
        await _transactionRepository.UpdateAsync(transaction);
        await _budgetRepository.UpdateAsync(budget);
        
        await UpdateBcSpending(budget.Id, transaction.Date, transaction.Category);
        
        return RedirectToAction("Edit", "Budget", new { id = transaction.BudgetId });
    }
    
    // GET: Transaction/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(transaction.BudgetId);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        return View(transaction);
    }
    
    // POST: Transaction/Delete/{id}
    [HttpPost, ActionName("DeleteTransaction")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        var budgetId = transaction.BudgetId;

        // Get the budget associated with the transaction
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Update the budget balance
        budget.Balance += transaction.Amount;
        await _budgetRepository.UpdateAsync(budget);
        
        var date = transaction.Date;
        var category = transaction.Category;
        
        // Delete the transaction
        if (await _transactionRepository.DeleteAsync(transaction))
        {
            await UpdateBcSpending(budgetId, date, category);
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }

        return RedirectToAction("Error", "Home");
    }
    
    private async Task UpdateBcSpending(int budgetId, DateTime transactionDate, TransactionCategory category)
    {
        // Get all budget categories associated with the budget
        var budgetCategories = await _budgetCategoryRepository.GetByBudgetIdAsync(budgetId);
        
        // Set the start and end date of the current month
        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        // Check if the transaction date is within the current month
        if (transactionDate < startDate || transactionDate > endDate)
        {
            // If the transaction date is not within the current month, return
            return;
        }
        
        foreach (var budgetCategory in budgetCategories)
        {
            // Get the transaction categories associated with the budget category
            var transactionCategories = await _bCtcRepository.GetTransactionCategoriesByBudgetCategoryIdAsync(budgetCategory.Id);
            
            // Check if the transaction category is associated with the budget category
            if (transactionCategories.Contains(category))
            {
                // Get the current month spending for the budget category
                var currentMonthSpending =
                    await _transactionRepository.GetCurrentMonthAmountForCategoriesAsync(budgetId,
                        transactionCategories);

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