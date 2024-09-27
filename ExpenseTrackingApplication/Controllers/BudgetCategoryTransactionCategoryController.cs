using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.Repositories;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class BudgetCategoryTransactionCategoryController : Controller
{
    private readonly IBudgetCategoryTransactionCategoryRepository _bCtcr;
    private readonly IBudgetCategoryRepository _budgetCategoryRepository;

    public BudgetCategoryTransactionCategoryController(IBudgetCategoryTransactionCategoryRepository bCtcr, IBudgetCategoryRepository budgetCategoryRepository)
    {
        _bCtcr = bCtcr;
        _budgetCategoryRepository = budgetCategoryRepository;
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
        
        var selectedCategories = await _bCtcr.GetByBudgetCategoryIdAsync(budgetCategoryId);
        
        var viewModel = new AssignTransactionCategoriesViewModel
        {
            BudgetCategoryId = budgetCategoryId,
            AllTransactionCategories = Enum.GetNames(typeof(TransactionCategory)).ToList(),
            SelectedCategories = selectedCategories.Select(bctc => bctc.TransactionCategory).ToList()
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

        var selectedCategories = viewModel.SelectedCategories ?? new List<string>();
        
        await _bCtcr.ClearByBudgetCategoryIdAsync(viewModel.BudgetCategoryId);

        foreach (var category in selectedCategories)
        {
            var bCtc = new BudgetCategoryTransactionCategory
            {
                BudgetCategoryId = viewModel.BudgetCategoryId,
                TransactionCategory = category
            };
            await _bCtcr.AddAsync(bCtc);
        }

        return RedirectToAction("Details", "Budget", new { id = budgetCategory.BudgetId });
    }
    
}