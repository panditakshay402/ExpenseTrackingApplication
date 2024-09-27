using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBudgetCategoryRepository
{
    Task<IEnumerable<BudgetCategory?>> GetAllAsync();
    Task<BudgetCategory?> GetByIdAsync(int id);
    Task<IEnumerable<BudgetCategory>> GetByBudgetIdAsync(int budgetId);
    Task<bool> AddAsync(BudgetCategory budgetCategory);
    Task<bool> DeleteAsync(BudgetCategory budgetCategory);
    Task<bool> UpdateAsync(BudgetCategory budgetCategory);
    Task<bool> SaveAsync();
}