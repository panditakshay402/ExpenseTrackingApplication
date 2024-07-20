﻿using ExpenseTrackingApplication.Data;
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
    
    public async Task<IEnumerable<Transaction>> GetTransactionByUserAsync(string userId)
    {
        return await _context.Transactions.Where(t => t.AppUserId == userId).ToListAsync();
    }
    
    public async Task<IEnumerable<Transaction>> GetTransactionByCategoryAsync(TransactionCategory category)
    {
        return await _context.Transactions.Where(t => t.Category == category).ToListAsync();
    }
    
    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions.Where(t => t.AppUserId == userId && t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }
    
    public async Task<decimal> GetTotalAmountByCategoryAsync(string userId, TransactionCategory category)
    {
        return (decimal)await _context.Transactions
            .Where(t => t.AppUserId == userId && t.Category == category)
            .SumAsync(t => t.Amount);
    }
    
    public async Task<int> GetTransactionCountByUserAsync(string userId)
    {
        return await _context.Transactions.CountAsync(t => t.AppUserId == userId);
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