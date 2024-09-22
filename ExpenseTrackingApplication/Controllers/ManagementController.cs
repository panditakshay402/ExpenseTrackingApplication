using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.Services;
using ExpenseTrackingApplication.ViewModels;
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
    private readonly INotificationService _notificationService;
    public ManagementController(IUserRepository userRepository, UserManager<AppUser> userManager, INotificationService notificationService)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _notificationService = notificationService;
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
    
    public async Task<IActionResult> BulkAction(string bulkAction, List<string> selectedUsers)
    {
        if (selectedUsers != null && selectedUsers.Count > 0)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (bulkAction == "block")
                    {
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTimeOffset.MaxValue;
                    }
                    else if (bulkAction == "unblock")
                    {
                        user.LockoutEnd = null;
                    }
                    else if (bulkAction == "notify")
                    {
                        // Implement the notification logic here
                    }
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        return RedirectToAction("ManageUsers");
    }
    
    [HttpGet]
    public IActionResult CreateBulkNotification(List<string> userIds)
    {
        var model = new NotificationBulkViewModel
        {
            UserIds = userIds
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBulkNotification(NotificationBulkViewModel model)
    {
        if (ModelState.IsValid)
        {
            foreach (var userId in model.UserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var notification = new Notification
                    {
                        AppUserId = user.Id,
                        Topic = model.Topic,
                        Message = model.Message,
                        Type = NotificationType.User,
                        Date = DateTime.UtcNow,
                        IsRead = false
                    };

                    await _notificationService.CreateAsync(notification);
                }
            }

            return RedirectToAction("ManageUsers", "Management");
        }

        return View(model);
    }

    
    
    public IActionResult ManageNotifications()
    {
        return View();
    }
}