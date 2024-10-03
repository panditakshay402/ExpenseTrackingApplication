using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<Budget?>> GetAllAsync();
    Task<Budget?> GetByIdAsync(int id);
    Task<IEnumerable<Budget>> GetBudgetByUserAsync(string userId);
    Task<bool> AddAsync(Budget budget);
    Task<bool> DeleteAsync(Budget budget);
    Task<bool> UpdateAsync(Budget budget);
    Task<bool> SaveAsync();
    Task<bool> ExistsAsync(int budgetId);
    Task<bool> UserOwnsBudgetAsync(int budgetId, string userId);
}