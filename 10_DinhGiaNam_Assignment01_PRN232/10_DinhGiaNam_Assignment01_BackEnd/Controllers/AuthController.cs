using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly SystemAccountRepository _repo;
    public AuthController(IConfiguration config, SystemAccountRepository repo) { _config = config; _repo = repo; }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var adminEmail = _config["AdminAccount:Email"];
        var adminPassword = _config["AdminAccount:Password"];
        if (request.Email == adminEmail && request.Password == adminPassword)
            return Ok(new LoginResponse { Name = "Administrator", Email = request.Email, Role = "Admin" });

        var account = await _repo.AuthenticateAsync(request.Email, request.Password);
        if (account == null) return Unauthorized(new { message = "Invalid email or password." });
        if (account.AccountRole != 1) return StatusCode(403, new { message = "Only Staff role can sign in to the management client." });
        return Ok(new LoginResponse { AccountId = account.AccountId, Name = account.AccountName ?? "Staff", Email = account.AccountEmail ?? "", Role = "Staff" });
    }
}
