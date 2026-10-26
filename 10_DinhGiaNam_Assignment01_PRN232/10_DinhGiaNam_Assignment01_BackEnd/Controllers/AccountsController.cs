using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly SystemAccountRepository _repo;
    public AccountsController(SystemAccountRepository repo) => _repo = repo;

    [HttpGet, EnableQuery]
    public async Task<IActionResult> Get([FromQuery] string? search = null)
    {
        if (!ControllerHelpers.IsAdmin(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        return Ok((await _repo.GetAllAsync(search)).AsQueryable());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOne(short id)
    {
        if (!(ControllerHelpers.IsAdmin(Request) || (ControllerHelpers.IsStaff(Request) && ControllerHelpers.UserId(Request) == id))) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        var e = await _repo.GetAsync(id); return e == null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccountUpsertDto dto)
    {
        if (!ControllerHelpers.IsAdmin(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var r = await _repo.CreateAsync(dto); return r.ok ? Ok(r.data) : BadRequest(new { message = r.message });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(short id, AccountUpsertDto dto)
    {
        var ownStaff = ControllerHelpers.IsStaff(Request) && ControllerHelpers.UserId(Request) == id;
        if (!(ControllerHelpers.IsAdmin(Request) || ownStaff)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (ownStaff) dto.AccountRole = 1;
        var r = await _repo.UpdateAsync(id, dto); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(short id)
    {
        if (!ControllerHelpers.IsAdmin(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        var r = await _repo.DeleteAsync(id); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }
}
