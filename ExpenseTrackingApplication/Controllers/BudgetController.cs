using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.BudgetViewModels;
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
    private readonly IExpenseRepository _expenseRepository;
    private readonly IIncomeRepository _incomeRepository;
    private readonly IBillRepository _billRepository;
    private readonly INotificationRepository _notificationRepository;
    public BudgetController(IBudgetRepository budgetRepository, IBudgetCategoryRepository budgetCategoryRepository, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IBillRepository billRepository, INotificationRepository notificationRepository)
    {
        _budgetRepository = budgetRepository;
        _budgetCategoryRepository = budgetCategoryRepository;
        _expenseRepository = expenseRepository;
        _incomeRepository = incomeRepository;
        _billRepository = billRepository;
        _notificationRepository = notificationRepository;
    }
    
    // GET: Budget/Overview
    public async Task<IActionResult> Overview()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound();
        }

        // Get all budgets that belong to the currently logged-in user
        var budgets = await _budgetRepository.GetBudgetByUserAsync(userId);

        var budgetOverviewList = new List<BudgetOverviewViewModel>();

        foreach (var budget in budgets)
        {
            var monthlyIncome = await _incomeRepository.GetBudgetMonthIncomeAsync(budget.Id);
            var monthlyExpense = await _expenseRepository.GetBudgetMonthExpenseAsync(budget.Id);
            var monthlyTotalExpenses = await _expenseRepository.GetBudgetMonthExpensesCountAsync(budget.Id);
            var monthlyTotalIncomes = await _incomeRepository.GetBudgetMonthIncomesCountAsync(budget.Id);
            var monthlyTotalTransactions = monthlyTotalExpenses + monthlyTotalIncomes;

            budgetOverviewList.Add(new BudgetOverviewViewModel
            {
                Id = budget.Id,
                Name = budget.Name,
                Balance = budget.Balance,
                MonthlyIncome = monthlyIncome,
                MonthlyExpense = monthlyExpense,
                MonthlyTotalTransactions = monthlyTotalTransactions,
                CreatedDate = budget.CreatedDate // Assuming you have this in your budget model
            });
        }

        return View(budgetOverviewList);
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
        var newBudgetNumber = budgets.Count() + 1; // Calculate next number
        
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
           await _notificationRepository.SendNotificationAsync(
                userId,
                "New Budget Created",
                $"A new budget {newBudgetName} has been successfully created.",
                NotificationType.Budget
            );
            
            return RedirectToAction(nameof(Edit), new { id = newBudget.Id });
        }

        return View("Index");
    }
    
    // GET: Budget/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        // Get the ID of the currently logged-in user
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(id);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }

        // Get expenses and incomes for the budget
        var expenses = await _expenseRepository.GetByBudgetAsync(id);
        var incomes = await _incomeRepository.GetByBudgetAsync(id);
        
        // Combine expenses and incomes into a single list
        var combinedEntries = expenses
            .Select(t => new CombinedEntryViewModel
            {
                Date = t.Date,
                Type = "Expense",
                RecipientOrSource = t.Recipient,
                Amount = t.Amount,
                Category = t.Category.ToString(),
                Description = t.Description
            })
            .Concat(incomes.Select(i => new CombinedEntryViewModel
            {
                Date = i.Date,
                Type = "Income",
                RecipientOrSource = i.Source,
                Amount = i.Amount,
                Category = i.Category.ToString(),
                Description = i.Description
            }))
            .OrderByDescending(e => e.Date) // Sort by date
            .ToList();
        
        // Get bills and send notifications to the user
        var bills = await _billRepository.GetByBudgetAsync(id);
        foreach (var bill in bills)
        {
            if (!bill.IsPaid)
            {
                // Notification for bills due in 3 days
                if (bill.DueDate.Date <= DateTime.Now.AddDays(3).Date && !bill.ReminderSent)
                {
                    await _notificationRepository.SendNotificationAsync(userId, "Bill Reminder", $"Your bill '{bill.Name}' is due in 3 days.", NotificationType.Bill);
                    bill.ReminderSent = true;
                    await _billRepository.UpdateAsync(bill);
                }

                // Notification for overdue bills
                if (bill.DueDate.Date < DateTime.Now.Date && !bill.OverdueReminderSent)
                {
                    await _notificationRepository.SendNotificationAsync(userId, "Overdue Bill Reminder", $"Your bill '{bill.Name}' is overdue!", NotificationType.Bill);
                    bill.OverdueReminderSent = true;
                    await _billRepository.UpdateAsync(bill);
                }
            }
        }
        // Sort bills by due date
        var sortedBills = bills.OrderBy(b => b.DueDate).ToList();
        
        // Update current amounts for budget categories before getting them
        var budgetCategories = await _budgetCategoryRepository.GetByBudgetIdAsync(id);
        // Get all budgets for the current user
        var allBudgets = (await _budgetRepository.GetBudgetByUserAsync(userId)).ToList();
        
        var viewModel = new BudgetDetailsViewModel
        {
            Budget = budget,
            CombinedEntries = combinedEntries,
            Bills = sortedBills,
            AllBudgets = allBudgets,
            BudgetCategories = budgetCategories,
            TotalExpenseAmount = expenses.Sum(t => t.Amount),
            TotalIncomeAmount = incomes.Sum(i => i.Amount),
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
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(id);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        var expenses = await _expenseRepository.GetByBudgetAsync(id);
        var incomes = await _incomeRepository.GetByBudgetAsync(id);
        var bills = await _billRepository.GetByBudgetAsync(id);
        
        var viewModel = new BudgetEditViewModel
        {
            Id = id,
            Name = budget.Name,
            Expenses = expenses,
            Incomes = incomes,
            Bills = bills
        };

        return View(viewModel);
    }

    // POST: Budget/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Budget budget)
    {
        if (id != budget.Id)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid)
        {
            // Fetch budget edit view model
            var budgetModel = await _budgetRepository.GetByIdAsync(id);
            var expenses = await _expenseRepository.GetByBudgetAsync(id);
            var incomes = await _incomeRepository.GetByBudgetAsync(id);
            var bills = await _billRepository.GetByBudgetAsync(id);

            var viewModel = new BudgetEditViewModel
            {
                Id = budgetModel.Id,
                Name = budgetModel.Name,
                Expenses = expenses,
                Incomes = incomes,
                Bills = bills
            };
            return View(viewModel);
        }

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

            return RedirectToAction(nameof(Edit), new { id = budget.Id });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _budgetRepository.ExistsAsync(budget.Id))
            {
                return NotFound();
            }
            throw; // Re-throw the exception for further handling or logging
        }
    }
    
    // GET: Budget/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        // Check if the user owns the budget
        var ownershipCheckResult = await CheckUserOwnership(id);
        if (ownershipCheckResult != null)
        {
            return ownershipCheckResult;
        }
        
        // Convert Budget to BudgetOverviewViewModel
        var budgetViewModel = new BudgetOverviewViewModel
        {
            Id = budget.Id,
            Name = budget.Name,
        };

        return PartialView("_DeleteBudgetPartialView", budgetViewModel);
    }
    
    // POST: Budget/Delete/{id}
    [HttpPost, ActionName("DeleteBudget")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound();
        }
        
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        
        await _budgetRepository.DeleteAsync(budget);
        await _notificationRepository.SendNotificationAsync(
            userId,
            "Budget Removed",
            $"{budget.Name} has been successfully removed.",
            NotificationType.Budget
        );
        
        return RedirectToAction("Overview");
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
    
    // GET: Budget/AboutBudgets
    [AllowAnonymous] // Allows non-logged-in users to access this page
    public IActionResult AboutBudgets()
    {
        return View();
    }
    
}