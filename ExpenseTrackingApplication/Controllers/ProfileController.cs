using ExpenseTrackingApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Profile
    public IActionResult Index()
    {
        return View();
    }
    
    
}