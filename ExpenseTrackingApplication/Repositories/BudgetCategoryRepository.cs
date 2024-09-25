using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
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
        
        public async Task<bool> UpdateCurrentAmountAsync(int budgetId)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Last day of the month

            var expenseCategories = await _context.BudgetCategories
                .Where(bc => bc.BudgetId == budgetId && bc.Type == BudgetCategoryType.Expense)
                .ToListAsync();

            foreach (var category in expenseCategories)
            {
                var totalExpenses = await _context.Transactions
                    .Where(t => t.BudgetId == budgetId && t.Date >= startOfMonth && t.Date <= endOfMonth) // Filter for current month
                    .SumAsync(t => t.Amount);

                category.CurrentAmount = totalExpenses; // Update CurrentAmount property

                _context.BudgetCategories.Update(category);
            }

            var incomeCategories = await _context.BudgetCategories
                .Where(bc => bc.BudgetId == budgetId && bc.Type == BudgetCategoryType.Income)
                .ToListAsync();

            foreach (var category in incomeCategories)
            {
                var totalIncome = await _context.Transactions
                    .Where(t => t.BudgetId == budgetId && t.Date >= startOfMonth && t.Date <= endOfMonth) // Filter for current month
                    .SumAsync(t => t.Amount);

                category.CurrentAmount = totalIncome; // Update CurrentAmount property

                _context.BudgetCategories.Update(category);
            }

            return await SaveAsync();
        }

        
        public async Task<bool> CheckExpensesExceedingLimitAsync(int budgetId)
        {
            var expenseCategories = await _context.BudgetCategories
                .Where(bc => bc.Type == BudgetCategoryType.Expense && bc.BudgetId == budgetId)
                .ToListAsync();

            foreach (var category in expenseCategories)
            {
                if (category.CurrentAmount > category.Limit)
                {
                    return true; // An expense category exceeds its limit
                }
            }

            return false; // No categories exceed their limit
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