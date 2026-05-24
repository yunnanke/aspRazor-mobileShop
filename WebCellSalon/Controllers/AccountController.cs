using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCellSalon.Models;
using WebCellSalon.Services;

namespace WebCellSalon.Controllers;

public class AccountController(AuthService authService) : Controller
{
    [AllowAnonymous]
    [HttpGet("/Login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [HttpPost("/Login")]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await authService.LoginAsync(model.Login, model.Password, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Не удалось выполнить вход.");
            return View(model);
        }

        await authService.SignInAsync(HttpContext, result);
        TempData["SuccessMessage"] = "Вход выполнен успешно.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("/Logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutAsync(HttpContext);
        return RedirectToAction("Index", "Home");
    }
}
