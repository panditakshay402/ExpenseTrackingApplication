using System.Security.Claims;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class TransactionController : Controller
{
    private readonly ApplicationDbContext _context;

    public TransactionController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Transaction
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var transactions = await _context.Transactions
            .Where(t => t.AppUserId == userId)
            .ToListAsync();
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
    public async Task<IActionResult> Create([Bind("Id,Amount,Description,Date")] Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            transaction.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(transaction);
    }
    
}