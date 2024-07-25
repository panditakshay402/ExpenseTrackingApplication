using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<AppUser?> GetByIdAsync(string id);
    Task<bool> AddAsync(AppUser user);
    Task<bool> UpdateAsync(AppUser user);
    Task<bool> DeleteAsync(AppUser user);
    Task<bool> SaveAsync();
}