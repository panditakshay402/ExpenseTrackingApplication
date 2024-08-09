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