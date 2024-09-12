using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void CreateNotification(string appUserId, string topic, string message, NotificationType type)
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

        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }
    
    public List<Notification> GetUserNotifications(string appUserId)
    {
        return _context.Notifications
            .Where(n => n.AppUserId == appUserId && !n.IsRead)
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
}