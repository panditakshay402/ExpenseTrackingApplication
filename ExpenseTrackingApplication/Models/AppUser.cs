﻿using ExpenseTrackingApplication.Data.Enum;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackingApplication.Models;

public class AppUser : IdentityUser
{
    // TODO: Change ICollections to private?
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public Currency PreferredCurrency { get; set; }
    public DateTime RegistrationDate { get; set; }
}