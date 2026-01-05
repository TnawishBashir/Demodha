using Demodha.Data.Identity;
using Demodha.Models;
using Demodha.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Demodha.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var vm = new UserProfileVM
        {
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UserProfileVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.FirstName = vm.FirstName.Trim();
        user.LastName = vm.LastName.Trim();

        user.PhoneNumber = string.IsNullOrWhiteSpace(vm.PhoneNumber) ? user.PhoneNumber : vm.PhoneNumber.Trim();

        var res = await _userManager.UpdateAsync(user);
        if (!res.Succeeded)
        {
            foreach (var e in res.Errors)
                ModelState.AddModelError("", e.Description);

            return View(vm);
        }

        TempData["success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
