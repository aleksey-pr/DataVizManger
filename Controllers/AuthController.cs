using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataVizManager.Models;
using DataVizManager.Database;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Data.Common;
using Microsoft.JSInterop;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController : Controller
{
  private readonly ApplicationDbContext _db;
  private readonly PasswordHasher<string> _passwordHasher;

  public AuthController(ApplicationDbContext db)
  {
    _db = db;
    _passwordHasher = new PasswordHasher<string>();
  }

  public IActionResult ForgotPasswordBasic() => View();
  public IActionResult LoginBasic() => View();
  public IActionResult RegisterBasic() => View();


  [HttpPost]
  public async Task<IActionResult> Login(LoginViewModel model, [FromServices] IConfiguration config)
  {
    var user = _db.Users.FirstOrDefault(user => user.Email == model.Email);
    if (user == null)
      return Unauthorized("Invalid email");
    var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, model.Password);
    if (result == PasswordVerificationResult.Failed)
    {
      return Unauthorized("Invalid Password");
      // ViewData["LoginError"] = "Invalid Password";
      // return View("LoginBasic", model);
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("role", user.Role)
    };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(config["Jwt:ExpireMinutes"])),
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    Console.WriteLine("token:" + token + ", tokenSTring: " + tokenString);
    //Save token in session
    Response.Cookies.Append("JwtToken", tokenString, new CookieOptions
    {
      HttpOnly = true,
      Secure = true, // only over HTTPS
      Expires = DateTimeOffset.UtcNow.AddMinutes(60) // match your JWT expiry
    });

    TempData["LoginSuccess"] = "LoginSuccess";
    return RedirectToAction("Index", "Dashboards");
  }

  [HttpPost]
  public async Task<IActionResult> Register(RegisterViewModel model)
  {
    if (!ModelState.IsValid)
    {
      ViewData["ErrorMessage"] = "Please fill in all required fields.";
      return View("RegisterBasic", model);
    }

    if (_db.Users.Any(user => user.Email == model.Email))
    {
      ViewData["ErrorMessage"] = "Email already exists";
      return View("RegisterBasic", model);
    }

    var hashedPassword = _passwordHasher.HashPassword(null, model.Password);
    var user = new User
    {
      Email = model.Email,
      PasswordHash = hashedPassword,
      UserName = model.Username
    };

    _db.Users.Add(user);
    await _db.SaveChangesAsync();

    TempData["SuccessMessage"] = "Successful Registration";
    return RedirectToAction("LoginBasic");
  }

  // private string HashPassword(string password)
  // {
  //   // simple demo hash (use ASP.NET Identity for production!)
  //   byte[] salt = RandomNumberGenerator.GetBytes(16);
  //   string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
  //       password: password,
  //       salt: salt,
  //       prf: KeyDerivationPrf.HMACSHA256,
  //       iterationCount: 10000,
  //       numBytesRequested: 32));
  //   return hashed;
  // }

  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    Response.Cookies.Delete("JwtToken");
    return RedirectToAction("Login");
  }
}
