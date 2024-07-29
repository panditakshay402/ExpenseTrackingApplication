using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTrackingApplication.Models;

public class BudgetCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }  = "Budget Category";

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be a positive number.")]
    public decimal Limit { get; set; }

    [ForeignKey("Budget")]
    public int BudgetId { get; set; }
    public Budget? Budget { get; set; }
}