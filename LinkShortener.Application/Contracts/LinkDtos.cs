namespace LinkShortener.Application.Contracts;

public record CreateLinkRequest(string TargetUrl, DateTime? ExpiresAt = null, string? CustomCode = null);
public record LinkResponse(Guid Id, string Code, string ShortUrl, string TargetUrl, DateTime CreatedAt, DateTime? ExpiresAt);

public record StatsResponse(
    Guid LinkId,
    string Code,
    long TotalClicks,
    IEnumerable<ClicksByDay> ByDay,
    IEnumerable<TopItem> TopReferrers,
    IEnumerable<TopItem> UtmSources,
    IEnumerable<TopItem> UtmCampaigns
);

public record ClicksByDay(DateOnly Day, long Count);
public record TopItem(string Key, long Count);