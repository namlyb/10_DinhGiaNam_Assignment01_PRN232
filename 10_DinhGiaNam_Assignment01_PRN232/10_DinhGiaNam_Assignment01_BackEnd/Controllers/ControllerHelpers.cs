using Microsoft.AspNetCore.Mvc;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

public static class ControllerHelpers
{
    public static bool IsAdmin(HttpRequest r) => string.Equals(r.Headers["X-User-Role"], "Admin", StringComparison.OrdinalIgnoreCase);
    public static bool IsStaff(HttpRequest r) => string.Equals(r.Headers["X-User-Role"], "Staff", StringComparison.OrdinalIgnoreCase);
    public static short? UserId(HttpRequest r) => short.TryParse(r.Headers["X-User-Id"], out var id) ? id : null;
}
