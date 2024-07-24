using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
