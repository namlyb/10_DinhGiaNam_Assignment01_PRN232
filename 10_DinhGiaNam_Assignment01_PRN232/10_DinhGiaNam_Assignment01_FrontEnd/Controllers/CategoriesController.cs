using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class CategoriesController : Controller
{
    private readonly ApiClient _api; public CategoriesController(ApiClient api) => _api = api;
    private bool Staff => HttpContext.Session.GetString("Role") == "Staff";
    public async Task<IActionResult> Index(string? search = null) { if (!Staff) return RedirectToAction("Login", "Auth"); ViewBag.Search = search; return View(await _api.GetAsync<List<CategoryVm>>("api/categories" + (string.IsNullOrWhiteSpace(search) ? "" : "?search=" + Uri.EscapeDataString(search))) ?? new()); }
    [HttpPost] public async Task<IActionResult> Create(CategoryVm vm) { if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<CategoryVm>(HttpMethod.Post, "api/categories", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Category created." : r.message; return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Edit(CategoryVm vm) { if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<object>(HttpMethod.Put, $"api/categories/{vm.CategoryId}", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Category updated." : r.message; return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Delete(short id) { if (!Staff) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); var r = await _api.SendAsync<object>(HttpMethod.Delete, $"api/categories/{id}"); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Category deleted." : r.message; return RedirectToAction(nameof(Index)); }
}
