using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class HomeController : Controller
{
    private readonly ApiClient _api; public HomeController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index(string? search = null)
    {
        var url = "api/news/public" + (string.IsNullOrWhiteSpace(search) ? "" : "?search=" + Uri.EscapeDataString(search));
        ViewBag.Search = search; return View(await _api.GetAsync<List<NewsVm>>(url) ?? new());
    }
}
