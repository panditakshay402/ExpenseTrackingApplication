using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Transaction?>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task<IEnumerable<Transaction>> GetByBudgetAsync(int budgetId)
    {
        return await _context.Transactions.Where(t => t.BudgetId == budgetId).ToListAsync();
    }
    
    public async Task<decimal> GetCurrentMonthAmountAsync(int budgetId)
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        return await _context.Transactions
            .Where(t => t.BudgetId == budgetId 
                        && t.Date.Month == currentMonth 
                        && t.Date.Year == currentYear)
            .SumAsync(t => t.Amount);
    }
    
    public async Task<decimal> GetCurrentMonthAmountForCategoriesAsync(int budgetId, List<TransactionCategory> transactionCategories)
    {
        // Get the start and end dates for the current month
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1); // End date is the last day of the current month

        // Calculate total spending for the specified budget ID, current month, and selected transaction categories
        return await _context.Transactions
            .Where(t => t.BudgetId == budgetId 
                        && t.Date >= startDate 
                        && t.Date <= endDate 
                        && transactionCategories.Contains(t.Category)) // Filter by transaction categories
            .SumAsync(t => t.Amount);
    }
    
    public async Task<bool> AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}