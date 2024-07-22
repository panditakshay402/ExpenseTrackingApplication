using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IIncomeRepository
{
    Task<IEnumerable<Income?>> GetAllAsync();
    Task<Income?> GetByIdAsync(int id);
    Task<IEnumerable<Income>> GetByUserAsync(string userId);
    Task<IEnumerable<Income>> GetByCategoryAsync(IncomeCategory category);
    Task<IEnumerable<Income>> GetByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<int> GetIncomeCountByUserAsync(string userId);
    Task<bool> AddAsync(Income income);
    Task<bool> DeleteAsync(Income income);
    Task<bool> UpdateAsync(Income income);
    Task<bool> SaveAsync();
}