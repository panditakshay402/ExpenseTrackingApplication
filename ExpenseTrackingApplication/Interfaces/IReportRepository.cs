using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<Report?>> GetAllAsync();
    Task<Report?> GetByIdAsync(int id);
    Task<IEnumerable<Report>> GetByUserAsync(string appUserId);
    Task<bool> AddAsync(Report report);
    Task<bool> DeleteAsync(Report report);
    Task<bool> UpdateAsync(Report report);
    Task<bool> SaveAsync();
}