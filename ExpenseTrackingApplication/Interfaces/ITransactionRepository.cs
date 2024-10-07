using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<IEnumerable<Transaction>> GetByBudgetAsync(int budgetId);
    Task<IEnumerable<Transaction>> GetTransactionsByCategoriesAsync(int budgetId, List<TransactionCategory> transactionCategories);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate);
    Task<decimal> GetBudgetMonthExpenseAsync(int budgetId);
    Task<int> GetBudgetMonthExpensesCountAsync(int budgetId);
    Task<decimal> GetCurrentMonthAmountForCategoriesAsync(int budgetId, List<TransactionCategory> transactionCategories);
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> SaveAsync();
}