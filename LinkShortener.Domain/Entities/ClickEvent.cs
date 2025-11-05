namespace LinkShortener.Domain.Entities;

public class ClickEvent
{
    public long Id { get; set; }
    public Guid LinkId { get; set; }
    public Link Link { get; set; } = default!;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
}