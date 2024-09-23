using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface IBillRepository
{
    Task<IEnumerable<Bill?>> GetAllAsync();
    Task<Bill?> GetByIdAsync(int id);
    Task<bool> AddAsync(Bill bill);
    Task<bool> DeleteAsync(Bill bill);
    Task<bool> UpdateAsync(Bill bill);
    Task<bool> SaveAsync();
}