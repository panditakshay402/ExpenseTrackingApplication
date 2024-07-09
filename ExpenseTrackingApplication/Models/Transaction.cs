﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Transaction
{
    [Key]
    public int? Id { get; set; }
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionCategory Category { get; set; }
    public string? Description { get; set; }
    
}