using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<IEnumerable<Transaction>> GetTransactionByUserAsync(string userId);
    Task<IEnumerable<Transaction>> GetTransactionByCategoryAsync(TransactionCategory category);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalAmountByCategoryAsync(string userId, TransactionCategory category);
    Task<int> GetTransactionCountByUserAsync(string userId);
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> SaveAsync();
}