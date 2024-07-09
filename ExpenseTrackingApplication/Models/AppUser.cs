using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackingApplication.Models;

public class AppUser : IdentityUser
{
    public ICollection<Transaction>? Transactions { get; set; }
    
}