using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels;

public class NotificationBulkViewModel
{
    public List<string> UserIds { get; set; }
    [Required]
    public string Topic { get; set; }
    [Required]
    public string Message { get; set; }
}