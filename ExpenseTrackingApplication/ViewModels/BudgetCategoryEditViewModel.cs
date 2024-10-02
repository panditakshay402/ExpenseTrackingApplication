using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetCategoryEditViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public string Name { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be positive.")]
    public decimal Limit { get; set; }
}
