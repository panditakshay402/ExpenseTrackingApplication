using System.Security.Claims;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackingApplication.Controllers;

public class BillController : Controller
{
    private readonly IBillRepository _billRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly INotificationRepository _notificationRepository;
    public BillController(IBillRepository billRepository, INotificationRepository notificationRepository, IBudgetRepository budgetRepository)
    {
        _billRepository = billRepository;
        _budgetRepository = budgetRepository;
        _notificationRepository = notificationRepository;
    }
    
    // GET: Bill/Create
    public IActionResult Create(int budgetId)
    {
        ViewBag.BudgetId = budgetId;
        return View();
    }
    
    // POST: Bill/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Name,Amount,DueDate,Frequency")] Bill bill)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to create the bill. Please correct the errors and try again.");
            return View(bill);
        }
        
        bill.BudgetId = budgetId;

        if (await _billRepository.AddAsync(bill))
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget != null)
            {
                await _budgetRepository.UpdateAsync(budget);
            }
                
            return RedirectToAction("Edit", "Budget", new { id = budgetId });
        }
        
        return RedirectToAction("Details", "Budget", new { id = budgetId });
    }
    
    // GET: Bill/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var bill = await _billRepository.GetByIdAsync(id.Value);
        if (bill == null)
        {
            return NotFound();
        }

        return View(bill);
    }
    
    // GET: Bill/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound();
        }
        
        var billViewModel = new BillEditViewModel
        {
            Name = bill.Name,
            Amount = bill.Amount,
            DueDate = bill.DueDate,
            Frequency = bill.Frequency,
            BudgetId = bill.BudgetId
        };
        
        return View(billViewModel);
    }
    
    // POST: Bill/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BillEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to update the bill. Please correct the errors and try again.");
            return View(viewModel);
        }
        
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound();
        }

        bill.Name = viewModel.Name;
        bill.Amount = viewModel.Amount;
        bill.DueDate = viewModel.DueDate;
        bill.Frequency = viewModel.Frequency;

        await _billRepository.UpdateAsync(bill);
        return RedirectToAction("Edit", "Budget", new { id = bill.BudgetId });
    }
    
    // GET: Bill/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound();
        }

        return View(bill);
    }
    
    // POST: Bill/Delete/{id}
    [HttpPost, ActionName("DeleteBill")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound();
        }
        
        if (await _billRepository.DeleteAsync(bill))
        {
            return RedirectToAction("Edit", "Budget", new { id = bill.BudgetId });
        }
        
        return View(bill);
    }
    
    // POST: Bill/Pay/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int id)
    {
        // TODO: Czy przy zapłaceniu ma powstawać transakcja?
        var bill = await _billRepository.GetByIdAsync(id);
        if (bill == null)
        {
            return NotFound();
        }

        // Set the bill as paid
        bill.IsPaid = true;

        // Update the bill in the database
        if (await _billRepository.UpdateAsync(bill))
        {
            // Send a notification to the user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationRepository.SendNotificationAsync(userId, "Bill Payment", $"Bill {bill.Name} has been paid.", NotificationType.Bill);

            // Check the frequency and create a new bill if it's recurring
            if (bill.Frequency != BillFrequency.None)
            {
                // Calculate the new due date based on the frequency
                DateTime newDueDate = bill.DueDate;
                switch (bill.Frequency)
                {
                    case BillFrequency.Weekly:
                        newDueDate = newDueDate.AddDays(7);
                        break;
                    case BillFrequency.Monthly:
                        newDueDate = newDueDate.AddMonths(1);
                        break;
                    case BillFrequency.Yearly:
                        newDueDate = newDueDate.AddYears(1);
                        break;
                }

                // Create a new bill based on the existing one
                var newBill = new Bill
                {
                    Name = bill.Name,
                    Amount = bill.Amount,
                    DueDate = newDueDate,
                    Frequency = bill.Frequency,
                    IsPaid = false,
                    ReminderSent = false,
                    OverdueReminderSent = false,
                    BudgetId = bill.BudgetId
                };

                // Add the new bill to the repository
                await _billRepository.AddAsync(newBill);
            }

            return RedirectToAction("Details", "Budget", new { id = bill.BudgetId });
        }

        return View("Error");
    }

    
}