using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Authorize(Roles = "Staff")]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryRepository _repo;
    public CategoriesController(CategoryRepository repo) => _repo = repo;

    [HttpGet, EnableQuery]
    public async Task<IActionResult> Get([FromQuery] string? search = null) => Ok((await _repo.GetAllAsync(search)).AsQueryable());

    [HttpPost]
    public async Task<IActionResult> Create(CategoryUpsertDto dto)
    {
        if (!ControllerHelpers.IsStaff(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        return Ok(await _repo.CreateAsync(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(short id, CategoryUpsertDto dto)
    {
        if (!ControllerHelpers.IsStaff(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var r = await _repo.UpdateAsync(id, dto); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(short id)
    {
        if (!ControllerHelpers.IsStaff(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        var r = await _repo.DeleteAsync(id); return r.ok ? Ok(new { message = r.message }) : BadRequest(new { message = r.message });
    }
}
