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
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
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
                    return RedirectToAction("Index", "Home");
                }
            }
            // Password is incorrect
            TempData["Error"] = "Wrong credentials. Please, try again";
            return View(loginViewModel);
        }
        // User is not found
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
            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
