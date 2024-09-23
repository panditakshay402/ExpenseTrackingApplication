using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class BillRepository : IBillRepository
{
    private readonly ApplicationDbContext _context;

    public BillRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Bill?>> GetAllAsync()
    {
        return await _context.Bills.ToListAsync();
    }

    public async Task<Bill?> GetByIdAsync(int id)
    {
        return await _context.Bills.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> AddAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(Bill bill)
    {
        _context.Bills.Remove(bill);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Bill bill)
    {
        _context.Bills.Update(bill);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}