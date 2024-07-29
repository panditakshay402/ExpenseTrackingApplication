using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }
    public string? Topic { get; set; }
    public string? Message { get; set; }
    public NotificationType Type { get; set; }
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
    
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
}