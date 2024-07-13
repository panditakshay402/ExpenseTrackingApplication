using ExpenseTrackingApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingApplication.Controllers;

public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Notifications
    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications.ToListAsync();
        return View(notifications);
    }
}