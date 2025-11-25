using LinkShortener.Domain.Entities;

namespace LinkShortener.Domain.Interfaces;

public interface ILinkRepository
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
    Task AddAsync(Link link, CancellationToken ct);
    Task<Link?> GetByCodeAsync(string code, CancellationToken ct);
    Task<Link?> GetByIdAsync(Guid id, CancellationToken ct);
}