using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class NewsController : Controller
{
    private readonly ApiClient _api; public NewsController(ApiClient api) => _api = api;
    private bool Staff => HttpContext.Session.GetString("Role") == "Staff";
    public async Task<IActionResult> Index(string? search = null, bool history = false)
    {
        if (!Staff) return RedirectToAction("Login", "Auth");
        var url = history ? "api/news/history" : "api/news" + (string.IsNullOrWhiteSpace(search) ? "" : "?search=" + Uri.EscapeDataString(search));
        return View(new NewsPageVm { News = await _api.GetAsync<List<NewsVm>>(url) ?? new(), Categories = await _api.GetAsync<List<CategoryVm>>("api/categories") ?? new(), Tags = await _api.GetAsync<List<TagVm>>("api/tags") ?? new(), Search = search });
    }
    [HttpPost]
    public async Task<IActionResult> Create(NewsVm vm)
    {
        if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<NewsVm>(HttpMethod.Post, "api/news", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "News created." : r.message; return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Edit(NewsVm vm)
    {
        if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<object>(HttpMethod.Put, $"api/news/{vm.NewsArticleId}", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "News updated." : r.message; return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<object>(HttpMethod.Delete, $"api/news/{id}"); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "News deleted." : r.message; return RedirectToAction(nameof(Index));
    }
}
