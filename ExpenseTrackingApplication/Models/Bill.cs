using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Bill
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "The Bill name must be at most 50 characters long.")]
    public string? Name { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "DueData is required.")]
    public DateTime DueDate { get; set; }
    [Required(ErrorMessage = "Frequency is required.")]
    public BillFrequency Frequency { get; set; }
    public bool IsPaid { get; set; }
    public bool ReminderSent { get; set; }
    public bool OverdueReminderSent { get; set; }
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}
