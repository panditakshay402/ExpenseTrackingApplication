using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;
    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Expense?>> GetAllAsync()
    {
        return await _context.Expenses.ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses.FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task<IEnumerable<Expense>> GetByBudgetAsync(int budgetId)
    {
        return await _context.Expenses.Where(t => t.BudgetId == budgetId).ToListAsync();
    }
    
    public async Task<IEnumerable<Expense>> GetExpensesByCategoriesAsync(int budgetId, List<ExpenseCategory> transactionCategories)
    {
        return await _context.Expenses
            .Where(t => t.BudgetId == budgetId && transactionCategories.Contains(t.Category))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Expense>> GetByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate)
    {
        return await _context.Expenses
            .Where(t => t.BudgetId == budgetId && t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }
    
    public async Task<decimal> GetBudgetMonthExpenseAsync(int budgetId)
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        return await _context.Expenses
            .Where(t => t.BudgetId == budgetId 
                        && t.Date.Month == currentMonth 
                        && t.Date.Year == currentYear)
            .SumAsync(t => t.Amount);
    }
    
    public async Task<int> GetBudgetMonthExpensesCountAsync(int budgetId)
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        return await _context.Expenses
            .Where(i => i.BudgetId == budgetId 
                        && i.Date.Month == currentMonth 
                        && i.Date.Year == currentYear)
            .CountAsync();
    }
    
    public async Task<decimal> GetCurrentMonthAmountForCategoriesAsync(int budgetId, List<ExpenseCategory> transactionCategories)
    {
        // Get the start and end dates for the current month
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1); // End date is the last day of the current month

        // Calculate total spending for the specified budget ID, current month, and selected transaction categories
        return await _context.Expenses
            .Where(t => t.BudgetId == budgetId 
                        && t.Date >= startDate 
                        && t.Date <= endDate 
                        && transactionCategories.Contains(t.Category)) // Filter by transaction categories
            .SumAsync(t => t.Amount);
    }
    
    public async Task<bool> AddAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(Expense expense)
    {
        _context.Expenses.Remove(expense);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}