using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class AccountsController : Controller
{
    private readonly ApiClient _api; public AccountsController(ApiClient api) => _api = api;
    private bool Admin => HttpContext.Session.GetString("Role") == "Admin";
    public async Task<IActionResult> Index(string? search = null)
    {
        if (!Admin) return RedirectToAction("Login", "Auth"); ViewBag.Search = search;
        return View(await _api.GetAsync<List<AccountVm>>("api/accounts" + (string.IsNullOrWhiteSpace(search) ? "" : "?search=" + Uri.EscapeDataString(search))) ?? new());
    }
    [HttpPost] public async Task<IActionResult> Create(AccountVm vm) { if (!Admin) return Forbid(); var r = await _api.SendAsync<AccountVm>(HttpMethod.Post, "api/accounts", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Account created." : r.message; return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Edit(AccountVm vm) { if (!Admin) return Forbid(); var r = await _api.SendAsync<object>(HttpMethod.Put, $"api/accounts/{vm.AccountId}", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Account updated." : r.message; return RedirectToAction(nameof(Index)); }
    [HttpPost] public async Task<IActionResult> Delete(short id) { if (!Admin) return Forbid(); var r = await _api.SendAsync<object>(HttpMethod.Delete, $"api/accounts/{id}"); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Account deleted." : r.message; return RedirectToAction(nameof(Index)); }
}
