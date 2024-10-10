using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels.TransactionViewModels;

public class BillEditViewModel
{
    public int? Id { get; set; }
    [StringLength(50, ErrorMessage = "The Bill name must be at most 50 characters long.")]
    public string Name { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "DueData is required.")]
    public DateTime DueDate { get; set; }
    [Required(ErrorMessage = "Frequency is required.")]
    public BillFrequency Frequency { get; set; }
    public int BudgetId { get; set; }
}