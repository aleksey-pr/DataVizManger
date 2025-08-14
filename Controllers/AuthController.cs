using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TEMPLATE.Models;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController : Controller
{
  public IActionResult ForgotPasswordBasic() => View();
  public IActionResult LoginBasic() => View();
  public IActionResult RegisterBasic() => View();


  [HttpPost]
  public async Task<IActionResult> Login(LoginViewModel model)
  {
    // Example: Replace with database check
    if (model.Email == "admin@example.com" && model.Password == "password")
    {
      var claims = new List<Claim>
          {
              new Claim(ClaimTypes.Name, model.Email)
          };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

      TempData["LoginMessage"] = "Login successful!";
      return RedirectToAction("Index", "Home");
    }

    TempData["LoginMessage"] = "Invalid login attempt.";
    return RedirectToAction("Login");
  }

  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Login");
  }
}
