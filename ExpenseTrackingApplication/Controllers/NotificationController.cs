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

    [HttpGet]
    public JsonResult GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = _notificationService.GetUserNotifications(userId);
        return Json(notifications);
    }
}
