using Microsoft.AspNetCore.Mvc;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly TagRepository _repo;
    public TagsController(TagRepository repo) => _repo = repo;
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _repo.GetAllAsync());
}
