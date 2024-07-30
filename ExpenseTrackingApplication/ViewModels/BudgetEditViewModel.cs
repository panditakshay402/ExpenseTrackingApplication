using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetEditViewModel
{
    public Budget Budget { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    public IEnumerable<Income> Incomes { get; set; }
}