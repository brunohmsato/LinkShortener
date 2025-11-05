using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Link> Links => Set<Link>();
    public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.Code).HasMaxLength(20);
            b.Property(x => x.TargetUrl).HasMaxLength(2048);
        });

        modelBuilder.Entity<ClickEvent>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.LinkId, x.OccurredAt });
            b.Property(x => x.Referrer).HasMaxLength(1024);
            b.Property(x => x.UserAgent).HasMaxLength(512);
            b.Property(x => x.Ip).HasMaxLength(64);
        });
    }
}