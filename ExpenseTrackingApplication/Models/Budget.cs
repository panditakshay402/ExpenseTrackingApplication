using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int? Id { get; set; }
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public float? Amount { get; set; }
    public float? Limit { get; set; }
    
}