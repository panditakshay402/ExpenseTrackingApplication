using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Notification
{
    [Key]
    public int? Id { get; set; }
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public string? Type { get; set; }
    public string? Message { get; set; }
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
    
}