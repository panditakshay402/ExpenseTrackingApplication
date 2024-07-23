using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class Budget
{
    [Key]
    public int? Id { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal? Amount { get; set; } = 0;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
    public decimal? Limit { get; set; } = 0;
    
    [ForeignKey("AppUser")]
    public string? AppUserId {  get; set; }
    public AppUser? AppUser { get; set; }
    
}