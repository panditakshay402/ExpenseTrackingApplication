using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Transaction
{
    [Key]
    public int? Id { get; set; }
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    //public TransactionType Type { get; set; }
    public string? Description { get; set; }
    
}