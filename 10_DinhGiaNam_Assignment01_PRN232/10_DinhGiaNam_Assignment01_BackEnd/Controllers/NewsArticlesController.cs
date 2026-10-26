using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/news")]
public class NewsArticlesController : ControllerBase
{
    private readonly NewsArticleRepository _repo;
    public NewsArticlesController(NewsArticleRepository repo) => _repo = repo;

    [HttpGet("public"), EnableQuery]
    public async Task<IActionResult> Public([FromQuery] string? search = null) => Ok((await _repo.GetAllAsync(search, activeOnly: true)).AsQueryable());

    [HttpGet("public/{id}")]
    public async Task<IActionResult> PublicOne(string id)
    {
        var e = await _repo.GetAsync(id, true); return e == null ? NotFound() : Ok(e);
    }

    [HttpGet, EnableQuery]
    public async Task<IActionResult> Get([FromQuery] string? search = null)
    {
        if (!ControllerHelpers.IsStaff(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        return Ok((await _repo.GetAllAsync(search)).AsQueryable());
    }

    [HttpGet("history"), EnableQuery]
    public async Task<IActionResult> History()
    {
        var id = ControllerHelpers.UserId(Request); if (!ControllerHelpers.IsStaff(Request) || !id.HasValue) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        return Ok((await _repo.GetAllAsync(createdById: id)).AsQueryable());
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsArticleUpsertDto dto)
    {
        var id = ControllerHelpers.UserId(Request); if (!ControllerHelpers.IsStaff(Request) || !id.HasValue) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var r = await _repo.CreateAsync(dto, id.Value); return r.ok ? Ok(r.data) : BadRequest(new { message = r.message });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, NewsArticleUpsertDto dto)
    {
        var uid = ControllerHelpers.UserId(Request); if (!ControllerHelpers.IsStaff(Request) || !uid.HasValue) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var r = await _repo.UpdateAsync(id, dto, uid.Value); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ControllerHelpers.IsStaff(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        var r = await _repo.DeleteAsync(id); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }
}
