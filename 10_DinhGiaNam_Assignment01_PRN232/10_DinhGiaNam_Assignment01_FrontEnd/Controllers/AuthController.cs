using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_FrontEnd.Models;
using _10_DinhGiaNam_Assignment01_FrontEnd.Services;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Controllers;
public class AuthController : Controller
{
    private readonly ApiClient _api; public AuthController(ApiClient api) => _api = api;
    [HttpGet] public IActionResult Login() => View(new LoginVm());
    [HttpPost] public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var r = await _api.SendAsync<LoginResponseVm>(HttpMethod.Post, "api/auth/login", vm);
        if (!r.ok || r.data == null) { ViewBag.Error = r.message; return View(vm); }
        HttpContext.Session.SetString("Role", r.data.Role); HttpContext.Session.SetString("Name", r.data.Name); HttpContext.Session.SetString("Email", r.data.Email);
        if (r.data.AccountId.HasValue) HttpContext.Session.SetString("AccountId", r.data.AccountId.Value.ToString());
        return RedirectToAction("Index", "Home");
    }
    public IActionResult Logout() { HttpContext.Session.Clear(); return RedirectToAction("Index", "Home"); }
}
