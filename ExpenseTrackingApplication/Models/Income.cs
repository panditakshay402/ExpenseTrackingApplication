using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Income
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string? Source { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public IncomeCategory Category { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}