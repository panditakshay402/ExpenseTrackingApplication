using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBudgetCategoryExpenseCategoryRepository
{
    Task<IEnumerable<BudgetCategoryExpenseCategory?>> GetAllAsync();
    Task<BudgetCategoryExpenseCategory?> GetByIdAsync(int id);
    Task<List<ExpenseCategory>> GetExpenseCategoriesByBudgetCategoryIdAsync(int budgetCategoryId);
    Task ClearByBudgetCategoryIdAsync(int budgetCategoryId);
    Task<bool> AddAsync(BudgetCategoryExpenseCategory bCtc);
    Task<bool> UpdateAsync(BudgetCategoryExpenseCategory bCtc);
    Task<bool> DeleteAsync(BudgetCategoryExpenseCategory bCtc);
}
