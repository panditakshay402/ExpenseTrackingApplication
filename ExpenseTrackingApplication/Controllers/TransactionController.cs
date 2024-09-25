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
    private readonly INotificationService _notificationService;
    public TransactionController(ITransactionRepository transactionRepository, IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, INotificationService notificationService)
    {
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _notificationService = notificationService;
        _budgetCategoryRepository = budgetCategoryRepository;
    }
    
    // GET: Transaction/Create
    public IActionResult Create(int budgetId)
    {
        ViewBag.BudgetId = budgetId;
        ViewBag.TransactionCategory = new SelectList(Enum.GetValues(typeof(TransactionCategory)).Cast<TransactionCategory>().ToList());
        return View();
    }
    
    // POST: Transaction/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Recipient,Amount,Date,Category,Description")] Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            transaction.BudgetId = budgetId;

            if (await _transactionRepository.AddAsync(transaction))
            {
                var budget = await _budgetRepository.GetByIdAsync(budgetId);
                if (budget != null)
                {
                    budget.Balance -= transaction.Amount;
                    await _budgetRepository.UpdateAsync(budget);
                    await _budgetCategoryRepository.UpdateCurrentAmountAsync(budgetId);
                    
                    // Check if any expense categories exceed their limits for this budget
                    if (await _budgetCategoryRepository.CheckExpensesExceedingLimitAsync(budgetId))
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        await _notificationService.SendNotificationAsync(
                            userId,
                            "Budget Limit Exceeded",
                            "One or more of your expense categories have exceeded their limits.",
                            NotificationType.Budget
                        );
                        
                    }
                }
                
                return RedirectToAction("Edit", "Budget", new { id = budgetId });
            }
        }
        ViewBag.BudgetId = budgetId;
        return View(transaction);
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

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");

        var budget = await _budgetRepository.GetByIdAsync(transaction.BudgetId);
        if (budget == null || budget.AppUserId != userId)
        {
            return NotFound();
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
    
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");
        var budget = await _budgetRepository.GetByIdAsync(transaction.BudgetId);
        if (budget == null || budget.AppUserId != userId)
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
        
        return RedirectToAction("Edit", "Budget", new { id = transaction.BudgetId });
    }
    
    // GET: Transaction/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var transactionDetails = await _transactionRepository.GetByIdAsync(id);
        if (transactionDetails == null)
        {
            return NotFound();
        }

        return View(transactionDetails);
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

        int budgetId = transaction.BudgetId;

        // Get the budget associated with the transaction
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
        {
            return NotFound();
        }

        // Update the budget balance
        budget.Balance += transaction.Amount;
        await _budgetRepository.UpdateAsync(budget);

        // Delete the transaction
        if (await _transactionRepository.DeleteAsync(transaction))
        {
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }

        return RedirectToAction("Error", "Home");
    }

}