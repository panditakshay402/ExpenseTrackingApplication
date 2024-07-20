using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IIncomeRepository
{
    Task<IEnumerable<Income?>> GetAllAsync();
    Task<Income?> GetByIdAsync(int id);
    Task<IEnumerable<Income>> GetTransactionByUserAsync(string userId);
    Task<IEnumerable<Income>> GetTransactionByCategoryAsync(IncomeCategory category);
    Task<IEnumerable<Income>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<int> GetTransactionCountByUserAsync(string userId);
    Task<bool> AddAsync(Income income);
    Task<bool> DeleteAsync(Income income);
    Task<bool> UpdateAsync(Income income);
    Task<bool> SaveAsync();
}