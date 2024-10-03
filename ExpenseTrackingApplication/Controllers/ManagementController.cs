using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.Services;
using ExpenseTrackingApplication.ViewModels;
using ExpenseTrackingApplication.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationRepository _notificationRepository;
    public ManagementController(IUserRepository userRepository, UserManager<AppUser> userManager, INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _notificationRepository = notificationRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var adminProfile = await _userRepository.GetByIdAsync(userId);
        return View(adminProfile);
    }

    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userList = users.Select(user => new ManageUsersViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            LastLogin = user.LastLogin ?? null,
            IsBlocked = user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now
        }).ToList();

        return View(userList);
    }
    
    public async Task<IActionResult> BlockUser(string userId, bool block)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            if (block)
            {
                // Block user
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
            }
            else
            {
                // Unblock user
                user.LockoutEnd = null;
            }
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction("ManageUsers");
    }
    
    public IActionResult ManageNotifications()
    {
        return View();
    }
}