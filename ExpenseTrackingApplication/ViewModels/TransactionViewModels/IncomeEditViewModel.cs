using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Data.Enum;

namespace ExpenseTrackingApplication.ViewModels.TransactionViewModels;

public class IncomeEditViewModel
{
    public int? Id { get; set; }
    [MaxLength(100)]
    public string? Source { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required(ErrorMessage = "Date is required.")]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Category is required.")]
    public IncomeCategory Category { get; set; }
    public string? Description { get; set; }
    public int BudgetId { get; set; }
}