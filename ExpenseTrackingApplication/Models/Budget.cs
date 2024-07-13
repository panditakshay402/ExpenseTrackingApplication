using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int? Id { get; set; }
    public float? Amount { get; set; }
    public float? Limit { get; set; }
    [ForeignKey("AppUserId")]
    public string? AppUserId { get; set; }
    
}