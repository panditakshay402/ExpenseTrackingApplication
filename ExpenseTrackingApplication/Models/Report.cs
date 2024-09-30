using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Report
{
    [Key]
    public int Id { get; set; }
    public ReportType Type { get; set; }
    public int BudgetId { get; set; }
    public string BudgetName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Data { get; set; }
    
    [ForeignKey("AppUser")]
    [MaxLength(450)]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
}