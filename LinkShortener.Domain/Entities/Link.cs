namespace LinkShortener.Domain.Entities;

public class Link
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = default!;
    public string TargetUrl { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public string? OwnerUserId { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ClickEvent> Clicks { get; set; } = new List<ClickEvent>();
}