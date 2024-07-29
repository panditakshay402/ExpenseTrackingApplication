using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Income
{
    [Key]
    public int Id { get; set; }
    public string? Source { get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public IncomeCategory Category { get; set; }
    public string? Description { get; set; }
    
    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}