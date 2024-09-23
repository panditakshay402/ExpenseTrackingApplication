﻿using System.Security.Claims;
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
        ViewBag.UserId = userId;
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

            if (await _notificationService.AddAsync(notification))
            {
                return RedirectToAction("Index");
            }
        }
        ViewBag.UserId = userId;
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
