using Microsoft.EntityFrameworkCore;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

public class TagRepository
{
    private readonly IDbContextFactory<FUNewsManagementContext> _factory;
    public TagRepository(IDbContextFactory<FUNewsManagementContext> factory) => _factory = factory;
    public async Task<List<Tag>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Tags.AsNoTracking().OrderBy(x => x.TagId).ToListAsync();
    }
}
