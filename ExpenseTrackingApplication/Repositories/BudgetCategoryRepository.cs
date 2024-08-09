using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories
{
    public class BudgetCategoryRepository : IBudgetCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BudgetCategory?>> GetAllAsync()
        {
            return await _context.BudgetCategories.ToListAsync();
        }

        public async Task<BudgetCategory?> GetByIdAsync(int id)
        {
            return await _context.BudgetCategories.FirstOrDefaultAsync(bc => bc.Id == id);
        }
        
        public async Task<IEnumerable<BudgetCategory>> GetByBudgetIdAsync(int budgetId)
        {
            return await _context.BudgetCategories
                .Where(c => c.BudgetId == budgetId)
                .ToListAsync();
        }
        
        public async Task<bool> UpdateBalanceAsync(int budgetId, decimal amount)
        {
            var budget = await _context.BudgetCategories.FindAsync(budgetId);
            if (budget == null) return false;

            budget.CurrentSpending += amount;
            return await SaveAsync();
        }

        
        public async Task<bool> AddAsync(BudgetCategory budgetCategory)
        {
            await _context.BudgetCategories.AddAsync(budgetCategory);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(BudgetCategory budgetCategory)
        {
            _context.BudgetCategories.Remove(budgetCategory);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(BudgetCategory budgetCategory)
        {
            _context.BudgetCategories.Update(budgetCategory);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}