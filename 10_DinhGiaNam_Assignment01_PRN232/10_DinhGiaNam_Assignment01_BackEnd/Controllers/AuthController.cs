using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        LoginResponse response;
        if (string.Equals(request.Email, _config["AdminAccount:Email"], StringComparison.OrdinalIgnoreCase)
            && request.Password == _config["AdminAccount:Password"])
            response = new LoginResponse { Name = "Administrator", Email = request.Email, Role = "Admin" };
        else
        {
            var account = await _repo.AuthenticateAsync(request.Email, request.Password);
            if (account == null) return Unauthorized(new { message = "Invalid email or password." });
            if (account.AccountRole != 1) return StatusCode(403, new { message = "Only Staff role may sign in." });
            response = new LoginResponse
            {
                AccountId = account.AccountId,
                Name = account.AccountName ?? "Staff",
                Email = account.AccountEmail ?? "",
                Role = "Staff"
            };
        }
        var expiration = DateTime.UtcNow.AddHours(2);
        var claims = new List<Claim>
        {
            new("sub", response.AccountId?.ToString() ?? "admin"),
            new("name", response.Name), new("email", response.Email), new("role", response.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "FUNewsManagement",
            audience: _config["Jwt:Audience"] ?? "FUNewsManagementClient",
            claims: claims, expires: expiration,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        response.Token = new JwtSecurityTokenHandler().WriteToken(token);
        response.ExpiresAtUtc = expiration;
        return Ok(response);
    }
}
