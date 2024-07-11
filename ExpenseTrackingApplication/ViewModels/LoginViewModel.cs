using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}