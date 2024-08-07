using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<IEnumerable<Transaction>> GetByBudgetAsync(int budgetId);
    Task<IEnumerable<Transaction>> GetByCategoryAsync(int budgetId, string subCategory);
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> SaveAsync();
}