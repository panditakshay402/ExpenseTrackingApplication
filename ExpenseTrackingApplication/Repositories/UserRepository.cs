using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<AppUser?> GetByIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> AddAsync(AppUser user)
    {
        await _context.Users.AddAsync(user);
        return await SaveAsync();
    }
    
    public async Task<bool> DeleteAsync(AppUser user)
    {
        _context.Users.Remove(user);
        return await SaveAsync();
    }
    
    public async Task<bool> UpdateAsync(AppUser user)
    {
        _context.Users.Update(user);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}