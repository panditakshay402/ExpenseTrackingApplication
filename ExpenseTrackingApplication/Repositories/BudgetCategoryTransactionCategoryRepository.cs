using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class BudgetCategoryTransactionCategoryRepository : IBudgetCategoryTransactionCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetCategoryTransactionCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<BudgetCategoryTransactionCategory?>> GetAllAsync()
    {
        return await _context.BudgetCategoryTransactionCategories.ToListAsync();
    }

    public async Task<BudgetCategoryTransactionCategory?> GetByIdAsync(int id)
    {
        return await _context.BudgetCategoryTransactionCategories.FirstOrDefaultAsync(bctc => bctc.Id == id);
    }

    public async Task<List<BudgetCategoryTransactionCategory>> GetCategoriesByBudgetCategoryIdAsync(int budgetCategoryId)
    {
        return await _context.BudgetCategoryTransactionCategories
            .Where(bctc => bctc.BudgetCategoryId == budgetCategoryId)
            .ToListAsync();
    }
    
    public async Task ClearByBudgetCategoryIdAsync(int budgetCategoryId)
    {
        var categoriesToRemove = await _context.BudgetCategoryTransactionCategories
            .Where(bctc => bctc.BudgetCategoryId == budgetCategoryId)
            .ToListAsync();

        _context.BudgetCategoryTransactionCategories.RemoveRange(categoriesToRemove);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> AddAsync(BudgetCategoryTransactionCategory bCtc)
    {
        await _context.BudgetCategoryTransactionCategories.AddAsync(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> DeleteAsync(BudgetCategoryTransactionCategory bCtc)
    {
        _context.BudgetCategoryTransactionCategories.Remove(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> UpdateAsync(BudgetCategoryTransactionCategory bCtc)
    {
        _context.BudgetCategoryTransactionCategories.Update(bCtc);
        return await _context.SaveChangesAsync() > 0;
    }
    
}
