using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly NewsArticleRepository _repo;
    public ReportsController(NewsArticleRepository repo) => _repo = repo;

    [HttpGet("news")]
    public async Task<IActionResult> News([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ControllerHelpers.IsAdmin(Request)) return StatusCode(
    StatusCodes.Status403Forbidden,
    new { message = "Forbidden" }
);
        if (startDate > endDate) return BadRequest(new { message = "StartDate must be earlier than or equal to EndDate." });
        return Ok(await _repo.ReportAsync(startDate, endDate));
    }
}
