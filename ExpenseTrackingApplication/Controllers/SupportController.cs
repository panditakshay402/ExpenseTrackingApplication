using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class SupportController : Controller
{
    // GET: Support/Index
    public IActionResult Index()
    {
        return View();
    }

    // GET: Support/Contact
    public IActionResult Contact()
    {
        return View();
    }

    // GET: Support/FAQ
    public IActionResult FAQ()
    {
        return View();
    }
}