using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Report
{
    [Key]
    public int Id { get; set; }
    public string? Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
    public List<Transaction>? Transactions { get; set; }
    
}