using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class BudgetCategoryExpenseCategoryRepository : IBudgetCategoryExpenseCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetCategoryExpenseCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<BudgetCategoryExpenseCategory?>> GetAllAsync()
    {
        return await _context.BudgetCategoryExpenseCategories.ToListAsync();
    }

    public async Task<BudgetCategoryExpenseCategory?> GetByIdAsync(int id)
    {
        return await _context.BudgetCategoryExpenseCategories.FirstOrDefaultAsync(bctc => bctc.Id == id);
    }
    
    public async Task<List<ExpenseCategory>> GetExpenseCategoriesByBudgetCategoryIdAsync(int budgetCategoryId)
    {
        var budgetCategoryTransactionCategories = await _context.BudgetCategoryExpenseCategories
            .Where(bctc => bctc.BudgetCategoryId == budgetCategoryId)
            .ToListAsync();

        // Return list of TransactionCategory
        return budgetCategoryTransactionCategories
            .Select(bctc => Enum.Parse<ExpenseCategory>(bctc.ExpenseCategory))
            .ToList();
    }
    
    public async Task ClearByBudgetCategoryIdAsync(int budgetCategoryId)
    {
        var categoriesToRemove = await _context.BudgetCategoryExpenseCategories
            .Where(bctc => bctc.BudgetCategoryId == budgetCategoryId)
            .ToListAsync();

        _context.BudgetCategoryExpenseCategories.RemoveRange(categoriesToRemove);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> AddAsync(BudgetCategoryExpenseCategory bCtc)
    {
        await _context.BudgetCategoryExpenseCategories.AddAsync(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> DeleteAsync(BudgetCategoryExpenseCategory bCtc)
    {
        _context.BudgetCategoryExpenseCategories.Remove(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> UpdateAsync(BudgetCategoryExpenseCategory bCtc)
    {
        _context.BudgetCategoryExpenseCategories.Update(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
}
