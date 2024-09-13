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
public class BudgetController : Controller
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly INotificationService _notificationService;
    public BudgetController(IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, ITransactionRepository transactionRepository, IIncomeRepository incomeRepository, INotificationService notificationService)
    {
        _budgetRepository = budgetRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
        _transactionRepository = transactionRepository;
        _incomeRepository = incomeRepository;
        _notificationService = notificationService;
    }
    
    // GET: Budget
    public async Task<IActionResult> Index()
    {
        // Get the ID of the currently logged-in user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null)
        {
            return NotFound();
        }
        
        // Get all budgets that belong to the currently logged-in user
        var budgets = await _budgetRepository.GetBudgetByUserAsync(userId);

        return View(budgets);
    }
    
    // GET: Budget/AddNewBudget
    public async Task<IActionResult> AddNewBudget()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null)
        {
            return NotFound();
        }

        // Get the current number of budgets
        var budgets = await _budgetRepository.GetBudgetByUserAsync(userId);
        int newBudgetNumber = budgets.Count() + 1; // Calculate next number
        
        var newBudgetName = $"Budget {newBudgetNumber}"; // Generate name

        var newBudget = new Budget
        {
            Name = newBudgetName,
            Balance = 0,
            AppUserId = userId
        };

        if (await _budgetRepository.AddAsync(newBudget))
        {
            // Create a notification for the user after successfully adding a budget
            _notificationService.CreateNotification(
                userId,
                "New Budget Created",
                $"A new budget '{newBudgetName}' has been successfully created.",
                NotificationType.Budget
            );
            
            return RedirectToAction(nameof(Index));
        }

        return View("Index"); // Redirect to Index in case of failure
    }
    
    // GET: Budget/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var budgetDetails = await _budgetRepository.GetByIdAsync(id);
        if (budgetDetails == null)
        {
            return NotFound();
        }
    
        return View(budgetDetails);
    }
    
    // POST: Budget/Delete/{id}
    [HttpPost, ActionName("DeleteBudget")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        // Remove related transactions
        foreach (var transaction in budget.Transactions.ToList())
        {
            await _transactionRepository.DeleteAsync(transaction);
        }

        // Remove related incomes
        foreach (var income in budget.Incomes.ToList())
        {
            await _incomeRepository.DeleteAsync(income);
        }
        
        await _budgetRepository.DeleteAsync(budget);
        return RedirectToAction("Index");
    }
    
    // GET: Budget/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }

        // Get transactions and incomes for the budget
        var transactions = await _transactionRepository.GetByBudgetAsync(id);
        var incomes = await _incomeRepository.GetByBudgetAsync(id);
        
        // Get all budgets for the current user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var allBudgets = (await _budgetRepository.GetBudgetByUserAsync(userId)).ToList();
        
        // Get budget categories for the budget
        var budgetCategories = await _budgetCategoryRepository.GetByBudgetIdAsync(id);
        
        var viewModel = new BudgetDetailsViewModel
        {
            Budget = budget,
            Transactions = transactions,
            Incomes = incomes,
            // TODO: Fix navigation to other budgets.
            AllBudgets = allBudgets,
            BudgetCategories = budgetCategories,
            BudgetSelectList = new SelectList(allBudgets, "Id", "Name", budget.Id)
        };

        return View(viewModel);
    }
    
    // GET: Budget/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }

        var transactions = await _transactionRepository.GetByBudgetAsync(id);
        var incomes = await _incomeRepository.GetByBudgetAsync(id);

        var viewModel = new BudgetEditViewModel
        {
            Budget = budget,
            Transactions = transactions,
            Incomes = incomes
        };

        return View(viewModel);
    }


    // POST: Budget/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Balance")] Budget budget)
    {
        if (id != budget.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var budgetToUpdate = await _budgetRepository.GetByIdAsync(budget.Id);
                if (budgetToUpdate == null)
                {
                    return NotFound();
                }

                budgetToUpdate.Name = budget.Name;

                await _budgetRepository.UpdateAsync(budgetToUpdate);
                await _budgetRepository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BudgetExists(budget.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Edit), new { id = budget.Id });
        }

        // Re-fetch transactions and incomes in case of error
        var viewModel = new BudgetEditViewModel
        {
            Budget = await _budgetRepository.GetByIdAsync(id),
            Transactions = await _transactionRepository.GetByBudgetAsync(id),
            Incomes = await _incomeRepository.GetByBudgetAsync(id)
        };
        return View(viewModel);
    }
    
    private async Task<bool> BudgetExists(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        return budget != null;
    }

}