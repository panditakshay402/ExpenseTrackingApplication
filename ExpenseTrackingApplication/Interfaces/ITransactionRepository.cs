using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAll();
    Task<Transaction?> GetById(int id);
    Task<IEnumerable<Transaction>> GetTransactionByUser(string userId);
    Task<IEnumerable<Transaction>> GetTransactionByCategory(TransactionCategory category);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRange(string userId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalAmountByCategory(string userId, TransactionCategory category);
    Task<int> GetTransactionCountByUser(string userId);
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> SaveAsync();
}