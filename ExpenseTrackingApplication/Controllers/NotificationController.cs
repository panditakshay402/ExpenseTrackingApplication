using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<AppUser> _userManager;

    public NotificationController(INotificationService notificationService, UserManager<AppUser> userManager)
    {
        _notificationService = notificationService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = _notificationService.GetUserNotifications(userId);
        return View(notifications);
    }
    
    [HttpGet]
    public IActionResult CreateSingleNotification(string userId)
    {
        var user = _userManager.FindByIdAsync(userId).Result;
        if (user == null)
        {
            return NotFound();
        }

        var model = new NotificationCreateViewModel
        {
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSingleNotification(NotificationCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

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
            return RedirectToAction("ManageUsers", "Management");
        }

        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int notificationId)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            await _notificationService.DeleteAsync(notification);
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _notificationService.SaveAsync();
        }

        return Json(new { success = true });
    }
    
    [HttpGet]
    public IActionResult GetUnreadNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { hasUnread = false });
        }

        var hasUnreadNotifications = _notificationService.HasUnreadNotifications(userId);

        return Json(new { hasUnread = hasUnreadNotifications });
    }
}
