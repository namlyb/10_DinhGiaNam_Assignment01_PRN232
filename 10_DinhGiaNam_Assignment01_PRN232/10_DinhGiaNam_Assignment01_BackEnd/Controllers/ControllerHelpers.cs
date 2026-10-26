using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

// Role/account-id can only come from validated claims in a signed JWT.
public static class ControllerHelpers
{
    public static bool IsAdmin(HttpRequest r) => r.HttpContext.User.IsInRole("Admin");
    public static bool IsStaff(HttpRequest r) => r.HttpContext.User.IsInRole("Staff");
    public static short? UserId(HttpRequest r) => short.TryParse(
        r.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? r.HttpContext.User.FindFirstValue("sub"),
        out var id) ? id : null;
}
