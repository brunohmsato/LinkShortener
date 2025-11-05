using LinkShortener.Application.Contracts;
using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Interfaces;

public interface ILinkService
{
    Task<LinkResponse> CreateAsync(CreateLinkRequest req, Func<string> baseUrl, CancellationToken ct);
    Task<Link?> GetByCodeAsync(string code, CancellationToken ct);
    Task RecordClickAsync(Link link, string? referrer, string? userAgent, string? ip, IDictionary<string, string?> utm, CancellationToken ct);
    Task<StatsResponse> GetStatsAsync(Guid linkId, CancellationToken ct);
}
