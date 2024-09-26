using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<IEnumerable<Transaction>> GetByBudgetAsync(int budgetId);
    Task<decimal> GetCurrentMonthAmountAsync(int budgetId);
    Task<decimal> GetCurrentMonthAmountForCategoriesAsync(int budgetId, List<TransactionCategory> transactionCategories);
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> SaveAsync();
}