using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingApplication.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context, IEmailSender emailSender, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Login()
    {
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
            // Ensure the IsBlocked message is shown before checking password
            if (user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now)
            {
                _logger.LogWarning($"Login failed: User {user.Email} is blocked.");
                TempData["Error"] = "Your account is blocked. Please contact support.";
                return View(loginViewModel);  // Return immediately to prevent overriding error message
            }
            
            // User is found, check password
            var passwordCheck= await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            if (passwordCheck.Succeeded)
            {
                // Update LastLogin timestamp
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                
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
            UserName = registerViewModel.UserName,
            Email = registerViewModel.EmailAddress,
            RegistrationDate = DateTime.UtcNow
        };
        var newUserResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);

        if (newUserResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            // Automatically create a budget for the new user
            var budget = new Budget
            {
                AppUserId = newUser.Id,
                Balance = 0,
            };
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            
            // var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, code }, protocol: HttpContext.Request.Scheme);
            //
            // await _emailSender.SendEmailAsync(registerViewModel.EmailAddress, "Confirm your email",
            //     $"Please confirm your account by clicking <a href='{callbackUrl}'>here</a>.");

            
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
    
    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        var confirmEmailViewModel = new EmailConfirmViewModel();

        if (userId == null || code == null)
        {
            confirmEmailViewModel.EmailConfirmed = false;
            confirmEmailViewModel.Message = "Invalid email confirmation request.";
            return View(confirmEmailViewModel);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            confirmEmailViewModel.EmailConfirmed = false;
            confirmEmailViewModel.Message = $"Unable to load user with ID '{userId}'.";
            return View(confirmEmailViewModel);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            confirmEmailViewModel.EmailConfirmed = true;
            confirmEmailViewModel.Message = "Thank you for confirming your email.";
        }
        else
        {
            confirmEmailViewModel.EmailConfirmed = false;
            confirmEmailViewModel.Message = "Error confirming your email.";
        }

        return View(confirmEmailViewModel);
    }
    
    [HttpGet]
    public IActionResult ResetPassword(string? token = null)
    {
        var model = new PasswordResetViewModel { Token = token };
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        if (model.Token != null)
        {
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
    
    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(PasswordForgotViewModel passwordForgotViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(passwordForgotViewModel);
        }

        var user = await _userManager.FindByEmailAsync(passwordForgotViewModel.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

        await _emailSender.SendEmailAsync(passwordForgotViewModel.Email, "Reset Password",
            $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.");

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }
}
