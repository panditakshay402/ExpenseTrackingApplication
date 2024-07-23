using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        var response = new LoginViewModel();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if(!ModelState.IsValid)
        {
            _logger.LogWarning("Login failed: Invalid model state.");
            return View(loginViewModel);
        }
        
        var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
        
        if(user != null)
        {
            // User is found, check password
            var passwordCheck= await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            if (passwordCheck.Succeeded)
            {
                // Password is correct, sign in
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} logged in successfully.");
                    return RedirectToAction("Index", "Home");
                }
            }
            // Password is incorrect
            _logger.LogWarning($"Login failed: Wrong credentials for user {user.Email}.");
            TempData["Error"] = "Wrong credentials. Please, try again";
            return View(loginViewModel);
        }
        // User is not found
        _logger.LogWarning($"Login failed: User with email {loginViewModel.Email} not found.");
        TempData["Error"] = "Wrong credentials. Please, try again";
        return View(loginViewModel);
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
        if (user != null)
        {
            // User already exists
            TempData["Error"] = "This email is already in use";
            return View(registerViewModel);
        }

        var newUser = new AppUser()
        {
            Email = registerViewModel.EmailAddress,
            UserName = registerViewModel.EmailAddress
        };
        var newUserResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);

        if (newUserResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            // Automatically create a budget for the new user
            var budget = new Budget
            {
                AppUserId = newUser.Id,
                Amount = 0,
                Limit = 0
            };
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created a new account with password and default budget.");

            await _signInManager.SignInAsync(newUser, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in newUserResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        _logger.LogWarning($"User registration failed for email {registerViewModel.EmailAddress}.");
        return View(registerViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
