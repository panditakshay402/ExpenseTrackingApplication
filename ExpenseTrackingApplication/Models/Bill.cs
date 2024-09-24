using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Bill
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    
    public DateTime DueDate { get; set; }
    public BillFrequency Frequency { get; set; }
    public bool IsPaid { get; set; }
    public bool ReminderSent { get; set; }
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}
