using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface INotificationService
{
    void CreateNotification(string appUserId, string topic, string message, NotificationType type);
    Task CreateAsync(Notification notification);
    List<Notification> GetUserNotifications(string appUserId);
    void MarkAsRead(int notificationId);
    bool HasUnreadNotifications(string appUserId);
    Task<Notification?> GetNotificationByIdAsync(int notificationId);
    Task<bool> DeleteAsync(Notification notification);
    Task<bool> SaveAsync();
}