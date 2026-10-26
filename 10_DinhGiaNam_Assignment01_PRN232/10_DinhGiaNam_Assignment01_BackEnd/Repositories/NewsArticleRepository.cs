using Microsoft.EntityFrameworkCore;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

public class NewsArticleRepository
{
    private readonly IDbContextFactory<FUNewsManagementContext> _factory;
    public NewsArticleRepository(IDbContextFactory<FUNewsManagementContext> factory) => _factory = factory;

    public async Task<List<NewsArticle>> GetAllAsync(string? search = null, bool activeOnly = false, short? createdById = null)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.NewsArticles.AsNoTracking().Include(x => x.Category).Include(x => x.CreatedBy).Include(x => x.Tags).AsQueryable();
        if (activeOnly) q = q.Where(x => x.NewsStatus == true);
        if (createdById.HasValue) q = q.Where(x => x.CreatedById == createdById);
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            q = q.Where(x => (x.NewsTitle ?? "").Contains(search) || x.Headline.Contains(search) || (x.NewsContent ?? "").Contains(search) || (x.NewsSource ?? "").Contains(search));
        }
        return await q.OrderByDescending(x => x.CreatedDate).ToListAsync();
    }

    public async Task<NewsArticle?> GetAsync(string id, bool activeOnly = false)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.NewsArticles.AsNoTracking().Include(x => x.Category).Include(x => x.CreatedBy).Include(x => x.Tags).AsQueryable();
        if (activeOnly) q = q.Where(x => x.NewsStatus == true);
        return await q.FirstOrDefaultAsync(x => x.NewsArticleId == id);
    }

    private static async Task<string> NextIdAsync(FUNewsManagementContext db)
    {
        var ids = await db.NewsArticles.Select(x => x.NewsArticleId).ToListAsync();
        var max = ids.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty(0).Max();
        return (max + 1).ToString();
    }

    public async Task<(bool ok, string message, NewsArticle? data)> CreateAsync(NewsArticleUpsertDto dto, short staffId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Categories.AnyAsync(x => x.CategoryId == dto.CategoryId)) return (false, "Category does not exist.", null);
        var tags = await db.Tags.Where(x => dto.TagIds.Contains(x.TagId)).ToListAsync();
        var e = new NewsArticle
        {
            NewsArticleId = await NextIdAsync(db),
            NewsTitle = dto.NewsTitle?.Trim(),
            Headline = dto.Headline.Trim(),
            CreatedDate = DateTime.Now,
            NewsContent = dto.NewsContent?.Trim(),
            NewsSource = dto.NewsSource?.Trim(),
            CategoryId = dto.CategoryId,
            NewsStatus = dto.NewsStatus,
            CreatedById = staffId,
            UpdatedById = staffId,
            ModifiedDate = DateTime.Now,
            Tags = tags
        };
        db.NewsArticles.Add(e); await db.SaveChangesAsync(); return (true, "Created successfully.", e);
    }

    public async Task<(bool ok, string message)> UpdateAsync(string id, NewsArticleUpsertDto dto, short staffId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var e = await db.NewsArticles.Include(x => x.Tags).FirstOrDefaultAsync(x => x.NewsArticleId == id);
        if (e == null) return (false, "News article not found.");
        if (!await db.Categories.AnyAsync(x => x.CategoryId == dto.CategoryId)) return (false, "Category does not exist.");
        var tags = await db.Tags.Where(x => dto.TagIds.Contains(x.TagId)).ToListAsync();
        e.NewsTitle = dto.NewsTitle?.Trim(); e.Headline = dto.Headline.Trim(); e.NewsContent = dto.NewsContent?.Trim(); e.NewsSource = dto.NewsSource?.Trim();
        e.CategoryId = dto.CategoryId; e.NewsStatus = dto.NewsStatus; e.UpdatedById = staffId; e.ModifiedDate = DateTime.Now;
        e.Tags.Clear(); foreach (var tag in tags) e.Tags.Add(tag);
        await db.SaveChangesAsync(); return (true, "Updated successfully.");
    }

    public async Task<(bool ok, string message)> DeleteAsync(string id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var e = await db.NewsArticles.Include(x => x.Tags).FirstOrDefaultAsync(x => x.NewsArticleId == id);
        if (e == null) return (false, "News article not found.");
        e.Tags.Clear(); db.NewsArticles.Remove(e); await db.SaveChangesAsync(); return (true, "Deleted successfully.");
    }

    public async Task<List<NewsArticle>> ReportAsync(DateTime start, DateTime end)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var endExclusive = end.Date.AddDays(1);
        return await db.NewsArticles.AsNoTracking().Include(x => x.Category).Include(x => x.CreatedBy).Include(x => x.Tags)
            .Where(x => x.CreatedDate >= start.Date && x.CreatedDate < endExclusive)
            .OrderByDescending(x => x.CreatedDate).ToListAsync();
    }
}
