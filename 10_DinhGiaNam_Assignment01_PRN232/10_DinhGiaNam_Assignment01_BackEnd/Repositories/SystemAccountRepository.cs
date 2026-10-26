using Microsoft.EntityFrameworkCore;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.DTOs;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;

namespace _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

public class SystemAccountRepository
{
    private readonly IDbContextFactory<FUNewsManagementContext> _factory;
    public SystemAccountRepository(IDbContextFactory<FUNewsManagementContext> factory) => _factory = factory;

    public async Task<List<SystemAccount>> GetAllAsync(string? search = null)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.SystemAccounts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            q = q.Where(x => (x.AccountName ?? "").Contains(search) || (x.AccountEmail ?? "").Contains(search));
        }
        return await q.OrderBy(x => x.AccountId).ToListAsync();
    }

    public async Task<SystemAccount?> GetAsync(short id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SystemAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == id);
    }

    public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SystemAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountEmail == email && x.AccountPassword == password);
    }

    public async Task<(bool ok, string message, SystemAccount? data)> CreateAsync(AccountUpsertDto dto)
    {
        await using var db = await _factory.CreateDbContextAsync();
        if (await db.SystemAccounts.AnyAsync(x => x.AccountEmail == dto.AccountEmail)) return (false, "Email already exists.", null);
        if (string.IsNullOrWhiteSpace(dto.AccountPassword) || dto.AccountPassword.Length < 2) return (false, "Password must have at least 2 characters.", null);
        short id = (short)((await db.SystemAccounts.MaxAsync(x => (short?)x.AccountId) ?? 0) + 1);
        var entity = new SystemAccount { AccountId = id, AccountName = dto.AccountName.Trim(), AccountEmail = dto.AccountEmail.Trim(), AccountRole = dto.AccountRole, AccountPassword = dto.AccountPassword };
        db.SystemAccounts.Add(entity);
        await db.SaveChangesAsync();
        return (true, "Created successfully.", entity);
    }

    public async Task<(bool ok, string message)> UpdateAsync(short id, AccountUpsertDto dto)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.SystemAccounts.FindAsync(id);
        if (entity == null) return (false, "Account not found.");
        if (await db.SystemAccounts.AnyAsync(x => x.AccountEmail == dto.AccountEmail && x.AccountId != id)) return (false, "Email already exists.");
        entity.AccountName = dto.AccountName.Trim(); entity.AccountEmail = dto.AccountEmail.Trim(); entity.AccountRole = dto.AccountRole; if (!string.IsNullOrWhiteSpace(dto.AccountPassword)) entity.AccountPassword = dto.AccountPassword;
        await db.SaveChangesAsync();
        return (true, "Updated successfully.");
    }

    public async Task<(bool ok, string message)> DeleteAsync(short id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.SystemAccounts.FindAsync(id);
        if (entity == null) return (false, "Account not found.");
        if (await db.NewsArticles.AnyAsync(x => x.CreatedById == id)) return (false, "Cannot delete this account because it has created news articles.");
        db.SystemAccounts.Remove(entity);
        await db.SaveChangesAsync();
        return (true, "Deleted successfully.");
    }
}
