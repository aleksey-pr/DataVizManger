using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using DataVizManager.Models;
using DataVizManager.Database;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  private readonly ApplicationDbContext _db;

  public DashboardsController(ApplicationDbContext db)
  {
    _db = db;
  }
  public IActionResult Index()
  {
    var users = _db.Users
        .Select(u => new { u.Id, u.UserName })
        .ToList();

    ViewData["Users"] = users;
    return View();
  }

  [HttpGet("/Dashboards/GetUserStats/{username}")]
  public IActionResult GetUserStats(String username)
  {
    var user = _db.Reports.FirstOrDefault(u => u.UserName == username);
    if (user == null) return NotFound();

    return Ok(new
    {
      VoiceCallCount = user.VoiceCallCount,
      VideoCallCount = user.VideoCallCount,
      MockInterviewCount = user.MockInterviewCount,
      AppliedJob = user.AppliedJob
    });
  }
}
