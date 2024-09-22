using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class NotificationBulkViewModel
{
    public List<string> UserIds { get; set; }
    [Required]
    public string Topic { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public NotificationType Type { get; set; }
}