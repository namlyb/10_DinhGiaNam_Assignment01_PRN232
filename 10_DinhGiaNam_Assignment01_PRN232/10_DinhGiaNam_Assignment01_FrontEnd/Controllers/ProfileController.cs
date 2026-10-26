using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class ProfileController : Controller
{
    private readonly ApiClient _api; public ProfileController(ApiClient api) => _api = api;
    public async Task<IActionResult> Index()
    {
        var id = HttpContext.Session.GetString("AccountId"); if (HttpContext.Session.GetString("Role") != "Staff" || string.IsNullOrEmpty(id)) return RedirectToAction("Login", "Auth");
        return View(await _api.GetAsync<AccountVm>($"api/accounts/{id}") ?? new AccountVm());
    }
    [HttpPost]
    public async Task<IActionResult> Save(AccountVm vm)
    {
        var id = HttpContext.Session.GetString("AccountId"); if (id != vm.AccountId.ToString()) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
); vm.AccountRole = 1; var r = await _api.SendAsync<object>(HttpMethod.Put, $"api/accounts/{vm.AccountId}", vm); TempData[r.ok ? "Ok" : "Error"] = r.ok ? "Profile updated." : r.message; return RedirectToAction(nameof(Index));
    }
}
