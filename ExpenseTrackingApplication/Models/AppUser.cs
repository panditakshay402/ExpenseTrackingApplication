using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackingApplication.Models;

public class AppUser : IdentityUser
{
    public ICollection<Budget> Budgets { get; init; } = new List<Budget>();
    public ICollection<Report> Reports { get; init; } = new List<Report>();
    public string? AvatarUrl { get; set; }
    public Currency PreferredCurrency { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsBlocked { get; set; }
}