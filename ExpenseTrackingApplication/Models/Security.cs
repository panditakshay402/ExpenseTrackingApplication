﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Security
{
    [Key]
    public int? Id { get; set; }
    
    // TODO: AppUserId or AppUser?
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
    private string? EncryptionMethod { get; set; }
    public DateTime LastLogin { get; set; }
    
}