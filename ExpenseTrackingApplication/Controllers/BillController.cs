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
    private readonly INotificationService _notificationService;
    public BillController(IBillRepository billRepository, INotificationService notificationService, IBudgetRepository budgetRepository)
    {
        _billRepository = billRepository;
        _budgetRepository = budgetRepository;
        _notificationService = notificationService;
    }
    
    // GET: Bill/Create
    public IActionResult Create(int budgetId)
    {
        ViewBag.BudgetId = budgetId;
        ViewBag.BillFrequency = new SelectList(Enum.GetValues(typeof(BillFrequency)).Cast<BillFrequency>().ToList());
        return View();
    }
    
    // POST: Bill/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int budgetId, [Bind("Name,Amount,DueDate,Frequency")] Bill bill)
    {
        if (ModelState.IsValid)
        {
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
        }
        ViewBag.BudgetId = budgetId;
        return View(bill);
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
        
        var viewModel = new BillEditViewModel
        {
            Name = bill.Name,
            Amount = bill.Amount,
            DueDate = bill.DueDate,
            Frequency = bill.Frequency
        };
        
        return View(viewModel);
    }
    
    // POST: Bill/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Amount,DueDate,Frequency")] BillEditViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            
            bill.Name = viewModel.Name;
            bill.Amount = viewModel.Amount;
            bill.DueDate = viewModel.DueDate;
            bill.Frequency = viewModel.Frequency;
            
            if (await _billRepository.UpdateAsync(bill))
            {
                return RedirectToAction("Details", new { id = bill.BudgetId });
            }
        }
        
        return View(viewModel);
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
            return RedirectToAction("Details", new { id = bill.BudgetId });
        }
        
        return View(bill);
    }
    
}