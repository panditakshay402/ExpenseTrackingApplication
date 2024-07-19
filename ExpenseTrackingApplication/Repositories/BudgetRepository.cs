using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;
    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Budget?>> GetAllAsync()
    {
        return await _context.Budgets.ToListAsync();
    }

    public async Task<Budget?> GetByIdAsync(int id)
    {
        return await _context.Budgets.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Budget>> GetBudgetByUserAsync(string userId)
    {
        return await _context.Budgets.Where(b => b.AppUserId == userId).ToListAsync();
    }

    public async Task<bool> AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        return true;
    }

    public async Task<bool> DeleteAsync(Budget budget)
    {
        _context.Budgets.Remove(budget);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}