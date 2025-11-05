using LinkShortener.Domain.Entities;

namespace LinkShortener.Domain.Interfaces;

public interface IClickEventRepository
{
    Task AddAsync(ClickEvent click, CancellationToken ct);
    IQueryable<ClickEvent> QueryByLinkId(Guid linkId);
}