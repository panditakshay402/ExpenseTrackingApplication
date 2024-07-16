using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
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
        var transactions = await _transactionRepository.GetTransactionByUser(userId);
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
            transaction.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? throw new ArgumentNullException(nameof(User), "User identifier not found");

            if (await _transactionRepository.AddAsync(transaction))
            {
                // SaveAsync should be called inside AddAsync to follow the unit of work pattern
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
        var transaction = await _transactionRepository.GetById(id.Value);

        if (transaction == null || transaction.AppUserId != userId)
        {
            return NotFound();
        }

        return View(transaction);
    }
}