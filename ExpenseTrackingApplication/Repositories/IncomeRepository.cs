using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly ApplicationDbContext _context;
    public IncomeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Income?>> GetAllAsync()
    {
        return await _context.Incomes.ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        return await _context.Incomes.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Income>> GetByBudgetAsync(int budgetId)
    {
        return await _context.Incomes.Where(i => i.BudgetId == budgetId).ToListAsync();
    }

    public async Task<bool> AddAsync(Income income)
    {
        await _context.Incomes.AddAsync(income);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(Income income)
    {
        _context.Incomes.Remove(income);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Income income)
    {
        _context.Incomes.Update(income);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}