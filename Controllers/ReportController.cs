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
    // IEnumerable<Reports> reportsList = _db.Reports;
    // return View(reportsList);
    var userName = User.Identity?.Name;

    // Filter reports for this user
    var reportsList = _db.Reports.Where(r => r.UserName == userName).ToList();

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

  public async Task<IActionResult> DeleteRow(int? id)
  {
    if (id == null || id == 0)
    {
      TempData["NotFound"] = "Invalid Report Id";
      return NotFound();
    }

    var report = await _db.Reports.FindAsync(id);
    if (report == null)
    {
      TempData["NotFound"] = "Invalid Report Id";
      return NotFound();
    }

    _db.Reports.Remove(report);
    await _db.SaveChangesAsync();

    TempData["DeleteSuccess"] = "Report deleted successfully";
    return RedirectToAction("Index");
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> EditRow(int id, Reports model)
  {
    if (id != model.Id)
    {
      TempData["NotFound"] = "Invalid Report Id";
      return NotFound();
    }
    Console.WriteLine("Id: " + model.Id + ", VoiceCallCount: " + model.VoiceCallCount + ", VideoCallCount: " + model.VideoCallCount + ", MockInterviewCount: " + model.MockInterviewCount + ", AppliedJob: " + model.AppliedJob);

    if (ModelState.IsValid)
    {
      var report = await _db.Reports.FindAsync(id);
      if (report == null)
      {
        TempData["NotFound"] = "Invalid Report Id";
        return NotFound();
      }

      // Update fields
      report.VoiceCallCount = model.VoiceCallCount;
      report.VideoCallCount = model.VideoCallCount;
      report.MockInterviewCount = model.MockInterviewCount;
      report.AppliedJob = model.AppliedJob;
      // Add other fields as needed

      await _db.SaveChangesAsync();
      TempData["EditSuccess"] = "Report updated successfully";
      return RedirectToAction("Index");
    }
    TempData["Invalid"] = "Invalid ModelState";
    return RedirectToAction("Index");
  }
}
