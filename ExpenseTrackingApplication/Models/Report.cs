using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Report
{
    [Key]
    public int Id { get; set; }
    public ReportType Type { get; set; }
    
    [MaxLength(100)]
    public string? ReportName { get; set; }
    public DateTime CreatedDate { get; set; }
    
    public string Data { get; set; }
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget Budget { get; set; }
}