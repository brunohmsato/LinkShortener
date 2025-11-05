using LinkShortener.Domain.Entities;
using LinkShortener.Domain.Interfaces;
using LinkShortener.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories;

public class ClickEventRepository(AppDbContext db) : IClickEventRepository
{
    public async Task AddAsync(ClickEvent click, CancellationToken ct)
        => await db.ClickEvents.AddAsync(click, ct);

    public IQueryable<ClickEvent> QueryByLinkId(Guid linkId)
        => db.ClickEvents
             .AsNoTracking()
             .Where(c => c.LinkId == linkId);
}