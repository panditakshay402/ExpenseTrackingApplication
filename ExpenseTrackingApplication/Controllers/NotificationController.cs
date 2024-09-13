using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = _notificationService.GetUserNotifications(userId);
        return View(notifications);
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
