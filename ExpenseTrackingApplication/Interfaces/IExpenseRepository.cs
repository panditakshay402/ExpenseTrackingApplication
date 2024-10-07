using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense?>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<IEnumerable<Expense>> GetByBudgetAsync(int budgetId);
    Task<IEnumerable<Expense>> GetExpensesByCategoriesAsync(int budgetId, List<ExpenseCategory> transactionCategories);
    Task<IEnumerable<Expense>> GetByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate);
    Task<decimal> GetBudgetMonthExpenseAsync(int budgetId);
    Task<int> GetBudgetMonthExpensesCountAsync(int budgetId);
    Task<decimal> GetCurrentMonthAmountForCategoriesAsync(int budgetId, List<ExpenseCategory> transactionCategories);
    Task<bool> AddAsync(Expense expense);
    Task<bool> DeleteAsync(Expense expense);
    Task<bool> UpdateAsync(Expense expense);
    Task<bool> SaveAsync();
}