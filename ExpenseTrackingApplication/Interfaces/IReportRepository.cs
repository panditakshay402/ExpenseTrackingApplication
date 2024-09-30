using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<Report?>> GetAllAsync();
    Task<Report?> GetByIdAsync(int id);
    Task<IEnumerable<Report>> GetByBudgetAsync(int budgetId);
    Task<IEnumerable<Report>> GetReportsByBudgetsAsync(IEnumerable<int> budgetIds);
    Task<bool> AddAsync(Report report);
    Task<bool> DeleteAsync(Report report);
    Task<bool> UpdateAsync(Report report);
    Task<bool> SaveAsync();
}