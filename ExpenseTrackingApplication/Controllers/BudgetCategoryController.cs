using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class BudgetCategoryController : Controller
{
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IIncomeRepository _incomeRepository;

    public BudgetCategoryController(IBudgetCategoryRepository budgetCategoryRepository, IBudgetRepository budgetRepository, ITransactionRepository transactionRepository, IIncomeRepository incomeRepository)
    {
        _budgetCategoryRepository = budgetCategoryRepository;
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        _incomeRepository = incomeRepository;
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
            Type = BudgetCategoryType.Expenses,
            CurrentBalance = 0,
            Limit = 0,
            BudgetId = budgetId
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

        ViewBag.IncomeCategories = Enum.GetValues(typeof(IncomeCategory)).Cast<IncomeCategory>().ToList();
        ViewBag.TransactionCategories = Enum.GetValues(typeof(TransactionCategory)).Cast<TransactionCategory>().ToList();

        return View(category);
    }

    // POST: BudgetCategory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetCategory category, string[] subCategories)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                category.SubCategory = string.Join(",", subCategories);
                
                // Calculate CurrentBalance
                category.CurrentBalance = await CalculateCurrentBalance(category);

                await _budgetCategoryRepository.UpdateAsync(category);
                return RedirectToAction("Details", "Budget", new { id = category.BudgetId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BudgetCategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        ViewBag.IncomeCategories = Enum.GetValues(typeof(IncomeCategory)).Cast<IncomeCategory>().ToList();
        ViewBag.TransactionCategories = Enum.GetValues(typeof(TransactionCategory)).Cast<TransactionCategory>().ToList();

        return View(category);
    }
    
    private async Task<decimal> CalculateCurrentBalance(BudgetCategory category)
    {
        if (category.Type == BudgetCategoryType.Incomes)
        {
            var incomes = await _incomeRepository.GetByCategoryAsync(category.BudgetId, category.SubCategory);
            return incomes.Sum(i => i.Amount);
        }
        else if (category.Type == BudgetCategoryType.Expenses)
        {
            var transactions = await _transactionRepository.GetByCategoryAsync(category.BudgetId, category.SubCategory);
            return transactions.Sum(t => t.Amount);
        }

        return 0;
    }

    private async Task<bool> BudgetCategoryExists(int id)
    {
        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        return category != null;
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
    
}