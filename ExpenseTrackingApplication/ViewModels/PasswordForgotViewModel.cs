using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels;

public class PasswordForgotViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
