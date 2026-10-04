using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class ReportController : Controller
{
    private readonly ApiClient _api; public ReportController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
    {
        if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login", "Auth");
        startDate ??= DateTime.Today.AddMonths(-1); endDate ??= DateTime.Today; ViewBag.Start = startDate.Value.ToString("yyyy-MM-dd"); ViewBag.End = endDate.Value.ToString("yyyy-MM-dd");
        return View(await _api.GetAsync<List<NewsVm>>($"api/reports/news?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}") ?? new());
    }
}
