using Microsoft.EntityFrameworkCore;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

public class CategoryRepository
{
    private readonly IDbContextFactory<FUNewsManagementContext> _factory;
    public CategoryRepository(IDbContextFactory<FUNewsManagementContext> factory) => _factory = factory;

    public async Task<List<Category>> GetAllAsync(string? search = null)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.Categories.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) { search = search.Trim(); q = q.Where(x => x.CategoryName.Contains(search) || x.CategoryDesciption.Contains(search)); }
        return await q.OrderBy(x => x.CategoryId).ToListAsync();
    }

    public async Task<Category?> GetAsync(short id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id);
    }

    public async Task<Category> CreateAsync(CategoryUpsertDto dto)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var e = new Category { CategoryName = dto.CategoryName.Trim(), CategoryDesciption = dto.CategoryDesciption.Trim(), ParentCategoryId = dto.ParentCategoryId, IsActive = dto.IsActive };
        db.Categories.Add(e); await db.SaveChangesAsync(); return e;
    }

    public async Task<(bool ok, string message)> UpdateAsync(short id, CategoryUpsertDto dto)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var e = await db.Categories.FindAsync(id); if (e == null) return (false, "Category not found.");
        e.CategoryName = dto.CategoryName.Trim(); e.CategoryDesciption = dto.CategoryDesciption.Trim(); e.ParentCategoryId = dto.ParentCategoryId; e.IsActive = dto.IsActive;
        await db.SaveChangesAsync(); return (true, "Updated successfully.");
    }

    public async Task<(bool ok, string message)> DeleteAsync(short id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var e = await db.Categories.FindAsync(id); if (e == null) return (false, "Category not found.");
        if (await db.NewsArticles.AnyAsync(x => x.CategoryId == id)) return (false, "Cannot delete this category because it is used by news articles.");
        if (await db.Categories.AnyAsync(x => x.ParentCategoryId == id && x.CategoryId != id)) return (false, "Cannot delete this category because it is a parent of another category.");
        db.Categories.Remove(e); await db.SaveChangesAsync(); return (true, "Deleted successfully.");
    }
}
