using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackingApplication.Models;
using ExpenseTrackingApplication.ViewModels;
using System.Threading.Tasks;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Interfaces;

namespace ExpenseTrackingApplication.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IPhotoService _photoService;
    private readonly INotificationRepository _notificationRepository;

    public UserController(IUserRepository userRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPhotoService photoService, INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _photoService = photoService;
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            RegistrationDate = user.RegistrationDate,
            AvatarUrl = user.AvatarUrl ?? "https://res.cloudinary.com/ggeztrw22/image/upload/v1722424290/avatars/default.jpg",
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserProfileEditViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl ?? "https://res.cloudinary.com/ggeztrw22/image/upload/v1722424290/avatars/default.jpg",
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserProfileEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        user.UserName = model.UserName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            
            // Send notification about successful profile update
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationRepository.SendNotificationAsync(userId, "Profile Update", "Your profile has been successfully updated.", NotificationType.User);

            
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAvatar()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        user.AvatarUrl = null;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            
            // Send notification about successful avatar remove
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _notificationRepository.SendNotificationAsync(userId, "Avatar Remove", "Your profile picture has been successfully removed.", NotificationType.User);
            
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "File not selected");
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var uploadResult = await _photoService.AddPhotoAsync(file);

        if (uploadResult.Error != null)
        {
            ModelState.AddModelError("", "Error uploading image");
            return RedirectToAction("Index");
        }

        user.AvatarUrl = uploadResult.SecureUrl.AbsoluteUri;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("Index");
    }
}
