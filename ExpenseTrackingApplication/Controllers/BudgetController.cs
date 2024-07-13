using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

public class BudgetController : Controller
{
    private readonly ApplicationDbContext _context;
    public BudgetController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Budget
    public async Task<IActionResult> Index()
    {
        return View(await _context.Budgets.ToListAsync());
    }
    
    // GET: Budget/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Budget/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Amount,Limit,AppUserId")] Budget budget)
    {
        if (ModelState.IsValid)
        {
            _context.Add(budget);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(budget);
    }
}