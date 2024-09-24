﻿using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.ViewModels;

public class BudgetDetailsViewModel
{
    public Budget Budget { get; set; }
    public IEnumerable<CombinedEntryViewModel> CombinedEntries { get; set; }
    public IEnumerable<Bill> Bills { get; set; }
    public IEnumerable<Budget> AllBudgets { get; set; }
    public IEnumerable<BudgetCategory> BudgetCategories { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public decimal TotalIncomeAmount { get; set; }
    public SelectList BudgetSelectList { get; set; }
}
