using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    
    // GET: Notification/CreateSingleNotification
    public IActionResult CreateSingleNotification(string userId)
    {   
        var user = _userManager.FindByIdAsync(userId).Result;
        if (user == null)
        {
            return NotFound();
        }
        
        ViewBag.UserId = userId;
        ViewBag.UserName = user.UserName;
        ViewBag.NotificationType = new SelectList(Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>().ToList());
    
        return View();
    }
    
    // POST: Notification/CreateSingleNotification
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSingleNotification(string userId, [Bind("Type,Topic,Message")] Notification notification)
    {
        if (ModelState.IsValid)
        {
            notification.AppUserId = userId;
            notification.Date = DateTime.UtcNow;
            notification.IsRead = false;
            
            if (await _notificationService.AddAsync(notification))
            {
                return RedirectToAction("ManageUsers", "Management");
            }
        }
        ViewBag.UserId = userId;
        return View(notification);
    }
    
    // GET: Notification/CreateAllNotification
    public IActionResult CreateAllNotification()
    {   
        ViewBag.NotificationType = new SelectList(Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>().ToList());
        return View();
    }
    
    // POST: Notification/CreateAllNotification
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAllNotification([Bind("Type,Topic,Message")] Notification notification)
    {
        if (ModelState.IsValid)
        {
            var users = await _userManager.Users.ToListAsync();
        
            foreach (var user in users)
            {
                var newNotification = new Notification
                {
                    AppUserId = user.Id,
                    Type = notification.Type,
                    Topic = notification.Topic,
                    Message = notification.Message,
                    Date = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationService.AddAsync(newNotification);
            }

            return RedirectToAction("ManageUsers", "Management");
        }

        ViewBag.NotificationType = new SelectList(Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>().ToList());
        return View(notification);
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
        
        return RedirectToAction("Index");
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
