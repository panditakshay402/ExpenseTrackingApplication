using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationRepository _notificationRepository;
    private readonly UserManager<AppUser> _userManager;

    public NotificationController(INotificationRepository notificationRepository, UserManager<AppUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = _notificationRepository.GetUserNotifications(userId);
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
            
            if (await _notificationRepository.AddAsync(notification))
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

                await _notificationRepository.AddAsync(newNotification);
            }

            return RedirectToAction("ManageUsers", "Management");
        }

        ViewBag.NotificationType = new SelectList(Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>().ToList());
        return View(notification);
    }

    
    [HttpPost]
    public async Task<IActionResult> Delete(int notificationId)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            await _notificationRepository.DeleteAsync(notification);
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _notificationRepository.SaveAsync();
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

        var hasUnreadNotifications = _notificationRepository.HasUnreadNotifications(userId);

        return Json(new { hasUnread = hasUnreadNotifications });
    }
    
}
