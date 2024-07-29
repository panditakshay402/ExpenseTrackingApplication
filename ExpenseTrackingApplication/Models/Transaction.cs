using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string? Recipient { get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionCategory Category { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    
    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}