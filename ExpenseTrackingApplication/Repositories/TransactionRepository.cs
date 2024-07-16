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
    public async Task<IEnumerable<Transaction?>> GetAll()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction?> GetById(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByCategory(TransactionCategory category)
    {
        return await _context.Transactions.Where(t => t.Category == category).ToListAsync();
    }

    public bool Add(Transaction transaction)
    {
        _context.Add(transaction);
        return Save();
    }
    
    public bool Delete(Transaction transaction)
    {
        _context.Remove(transaction);
        return Save();
    }
    
    public bool Update(Transaction transaction)
    {
        _context.Update(transaction);
        return Save();
    }
    
    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}