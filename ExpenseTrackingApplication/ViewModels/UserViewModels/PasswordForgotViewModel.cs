using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels.UserViewModels;

public class PasswordForgotViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
