using System.ComponentModel.DataAnnotations;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels.BudgetViewModels;

public class BudgetEditViewModel
{
    public int? Id { get; set; }
    [Required (ErrorMessage = "Name is required.")]
    [StringLength(25, ErrorMessage = "The Budget name must be at most 25 characters long.")]
    public string Name { get; set; } = "Budget";
    public IEnumerable<Expense> Expenses { get; set; } = new List<Expense>();
    public IEnumerable<Income> Incomes { get; set; } = new List<Income>();
    public IEnumerable<Bill> Bills { get; set; } = new List<Bill>();
}