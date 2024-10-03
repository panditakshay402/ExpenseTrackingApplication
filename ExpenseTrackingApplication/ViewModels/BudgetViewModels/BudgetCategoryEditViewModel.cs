using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetCategoryEditViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "The Budget Category name must be at most 50 characters long.")]
    public string Name { get; set; } = "Budget Category";
    [Range(0, double.MaxValue, ErrorMessage = "Limit must be positive.")]
    public decimal Limit { get; set; }
}
