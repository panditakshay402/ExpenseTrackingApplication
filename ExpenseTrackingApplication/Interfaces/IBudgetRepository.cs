using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<Budget?>> GetAll();
    Task<Budget?> GetById(int id);
    Task<IEnumerable<Budget>> GetBudgetByUser(string userId);
    Task<bool> AddAsync(Budget budget);
    Task<bool> DeleteAsync(Budget budget);
    Task<bool> UpdateAsync(Budget budget);
    Task<bool> SaveAsync();
}