using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface INotificationService
{
    void CreateNotification(string appUserId, string topic, string message, NotificationType type);
    List<Notification> GetUserNotifications(string appUserId);
    void MarkAsRead(int notificationId);
}