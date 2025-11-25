using LinkShortener.Domain.Entities;
using LinkShortener.Domain.Interfaces;
using LinkShortener.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories;

public class LinkRepository(AppDbContext db) : ILinkRepository
{
    public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct) =>
        db.Links.AnyAsync(x => x.Code == code, ct);

    public async Task AddAsync(Link link, CancellationToken ct) =>
        await db.Links.AddAsync(link, ct);

    public Task<Link?> GetByCodeAsync(string code, CancellationToken ct) =>
        db.Links.FirstOrDefaultAsync(x => x.Code == code && x.IsActive, ct);

    public Task<Link?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Links.FirstOrDefaultAsync(x => x.Id == id, ct);
}