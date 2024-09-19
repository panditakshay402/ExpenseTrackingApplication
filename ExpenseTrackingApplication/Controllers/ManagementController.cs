using System.Security.Claims;
using ExpenseTrackingApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    private readonly IUserRepository _userRepository;
    
    public ManagementController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var adminProfile = await _userRepository.GetByIdAsync(userId);
        return View(adminProfile);
    }

    public IActionResult ManageUsers()
    {
        return View();
    }

    public IActionResult ManageNotifications()
    {
        return View();
    }
}