using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using DataVizManager.Database;
using DataVizManager.Models;

namespace AspnetCoreMvcFull.Controllers;

public class ReportController : Controller
{
  private readonly ApplicationDbContext _db;

  public ReportController(ApplicationDbContext db)
  {
    _db = db;
  }

  public IActionResult Index()
  {
    IEnumerable<Reports> reportsList = _db.Reports;
    return View(reportsList);
  }

  [HttpPost]
  public async Task<IActionResult> SaveWR(ReportViewModel model)
  {
    // if (!ModelState.IsValid)
    // {
    //   TempData["Invalid"] = "invalid model state";
    //   return RedirectToAction("Index", "Report");
    // }

    // if (_db.Users.Any(user => user.Email == model.Email))
    // {
    //   ViewData["ErrorMessage"] = "Email already exists";
    //   return View("RegisterBasic", model);
    // }

    var report = new Reports
    {
      VoiceCallCount = model.VoiceCallCount,
      VideoCallCount = model.VideoCallCount,
      MockInterviewCount = model.MockInterviewCount,
      AppliedJob = model.AppliedJobCount,
      UserName = model.UserName
    };

    _db.Reports.Add(report);
    await _db.SaveChangesAsync();

    TempData["WRSaved"] = "WRSaved";
    return RedirectToAction("Index", "Report");
  }
}
