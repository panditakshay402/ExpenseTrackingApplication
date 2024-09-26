using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBudgetCategoryTransactionCategoryRepository
{
    Task<IEnumerable<BudgetCategoryTransactionCategory?>> GetAllAsync();
    Task<BudgetCategoryTransactionCategory?> GetByIdAsync(int id);
    Task<List<BudgetCategoryTransactionCategory>> GetByBudgetCategoryIdAsync(int budgetCategoryId);
    Task<bool> AddAsync(BudgetCategoryTransactionCategory bCtc);
    Task<bool> UpdateAsync(BudgetCategoryTransactionCategory bCtc);
    Task<bool> DeleteAsync(BudgetCategoryTransactionCategory bCtc);
}
