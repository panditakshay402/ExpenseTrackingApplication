using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;
    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Report?>> GetAllAsync()
    {
        return await _context.Reports.ToListAsync();
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        return await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Report>> GetByUserAsync(string appUserId)
    {
        return await _context.Reports.Where(r => r.AppUserId == appUserId).ToListAsync();
    }

    public async Task<bool> AddAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(Report report)
    {
        _context.Reports.Remove(report);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(Report report)
    {
        _context.Reports.Update(report);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}