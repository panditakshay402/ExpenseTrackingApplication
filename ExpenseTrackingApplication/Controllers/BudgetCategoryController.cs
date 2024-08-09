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
            Type = BudgetCategoryType.Expense,
            CurrentSpending = 0,
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

        // Załaduj dane potrzebne do widoku edycji
        var viewModel = new BudgetCategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            CurrentSpending = category.CurrentSpending,
            Limit = category.Limit,
            BudgetId = category.BudgetId
        };
        
        // Przygotuj listę typów kategorii dla rozwijanego menu
        ViewBag.CategoryTypes = new SelectList(Enum.GetValues(typeof(BudgetCategoryType))
                .Cast<BudgetCategoryType>()
                .Select(t => new { Value = t, Text = t.ToString() }),
            "Value", "Text", viewModel.Type);
        
        return View(viewModel);
    }

    // POST: BudgetCategory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BudgetCategoryEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest(); // lub inny odpowiedni kod błędu
        }

        var category = await _budgetCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        if (viewModel.Type == BudgetCategoryType.Expense)
        {
            // Pobierz sumę wydatków z bieżącego miesiąca
            viewModel.CurrentSpending = await _transactionRepository
                .GetCurrentMonthAmountAsync(category.BudgetId);
        }
        else if (viewModel.Type == BudgetCategoryType.Income)
        {
            // Pobierz sumę przychodów z bieżącego miesiąca
            viewModel.CurrentSpending = await _incomeRepository
                .GetCurrentMonthAmountAsync(category.BudgetId);
        }

        if (ModelState.IsValid)
        {
            // Zaktualizuj kategorię budżetową na podstawie modelu widoku
            category.Name = viewModel.Name;
            category.Limit = viewModel.Limit;
            category.Type = viewModel.Type;
            category.CurrentSpending = viewModel.CurrentSpending; // Aktualizuj CurrentSpending na podstawie wartości z widoku

            if (await _budgetCategoryRepository.UpdateAsync(category))
            {
                return RedirectToAction("Details", "Budget", new { id = category.BudgetId });
            }

            ModelState.AddModelError("", "Error while updating category.");
        }

        // Jeśli model nie jest ważny, zwróć widok z błędami walidacji
        return View(viewModel);
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