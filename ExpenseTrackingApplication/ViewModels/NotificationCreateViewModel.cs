using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels;

public class NotificationCreateViewModel
{
    public string Email { get; set; }
    [Required]
    public string Topic { get; set; }
    [Required]
    public string Message { get; set; }

}