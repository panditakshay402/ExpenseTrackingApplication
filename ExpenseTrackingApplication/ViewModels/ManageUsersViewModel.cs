namespace ExpenseTrackingApplication.ViewModels;

public class ManageUsersViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsBlocked { get; set; }
}