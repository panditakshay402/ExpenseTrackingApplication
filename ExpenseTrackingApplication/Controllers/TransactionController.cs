﻿using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class TransactionController : Controller
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    // GET: Transaction
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");
        var transactions = await _transactionRepository.GetByUserAsync(userId);
        return View(transactions);
    }
    
    // GET: Transaction/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Transaction/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Recipient,Amount,Date,Category,Description")] Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            transaction.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? throw new ArgumentNullException(nameof(User), "User identifier not found");

            if (await _transactionRepository.AddAsync(transaction))
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View(transaction);
    }
    
    // GET: Transaction/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? throw new ArgumentNullException(nameof(User), "User identifier not found");
        var transaction = await _transactionRepository.GetByIdAsync(id.Value);

        if (transaction == null || transaction.AppUserId != userId)
        {
            return NotFound();
        }

        return View(transaction);
    }
    
    // GET: Transaction/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        var transactionViewModel = new EditTransactionViewModel
        {
            Id = transaction.Id,
            Recipient = transaction.Recipient,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Category = transaction.Category,
            Description = transaction.Description,
            AppUserId = transaction.AppUserId
        };

        return View(transactionViewModel);
    }
    
    // POST: Transaction/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditTransactionViewModel transactionViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit transaction.");
            return View("Edit", transactionViewModel);
        }

        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
    
        transaction.Recipient = transactionViewModel.Recipient;
        transaction.Amount = transactionViewModel.Amount;
        transaction.Date = transactionViewModel.Date;
        transaction.Category = transactionViewModel.Category;
        transaction.Description = transactionViewModel.Description;

        await _transactionRepository.UpdateAsync(transaction);

        return RedirectToAction("Index");
    }
    
    // GET: Transaction/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var transactionDetails = await _transactionRepository.GetByIdAsync(id);
        if (transactionDetails == null)
        {
            return NotFound();
        }

        return View(transactionDetails);
    }
    
    // POST: Transaction/Delete/{id}
    [HttpPost, ActionName("DeleteTransaction")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        await _transactionRepository.DeleteAsync(transaction);
        return RedirectToAction("Index");
    }
}