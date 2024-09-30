using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SendNotificationAsync(string appUserId, string topic, string message, NotificationType type)
    {
        var notification = new Notification
        {
            Topic = topic,
            Message = message,
            Type = type,
            Date = DateTime.Now,
            IsRead = false,
            AppUserId = appUserId
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    
    public async Task<bool> AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        return await SaveAsync();
    }
    
    public List<Notification> GetUserNotifications(string appUserId)
    {
        return _context.Notifications
            .Where(n => n.AppUserId == appUserId)
            .ToList();
    }


    public void MarkAsRead(int notificationId)
    {
        var notification = _context.Notifications.Find(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            _context.SaveChanges();
        }
    }
    
    // Check if the user has unread notifications
    public bool HasUnreadNotifications(string appUserId)
    {
        return _context.Notifications
            .Any(n => n.AppUserId == appUserId && !n.IsRead);
    }
    
    public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
    {
        return await _context.Notifications.FindAsync(notificationId);
    }
    
    public async Task<bool> DeleteAsync(Notification notification)
    {
        _context.Notifications.Remove(notification);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}