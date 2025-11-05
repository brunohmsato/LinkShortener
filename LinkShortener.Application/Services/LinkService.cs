using LinkShortener.Application.Contracts;
using LinkShortener.Application.Interfaces;
using LinkShortener.Domain.Abstractions;
using LinkShortener.Domain.Entities;
using LinkShortener.Domain.Interfaces;
using System.Security.Cryptography;

namespace LinkShortener.Application.Services;

public class LinkService(
    ILinkRepository links,
    IClickEventRepository clicks,
    IUnitOfWork uow) : ILinkService
{
    public async Task<LinkResponse> CreateAsync(CreateLinkRequest req, Func<string> baseUrl, CancellationToken ct)
    {
        var code = string.IsNullOrWhiteSpace(req.CustomCode) ?
            await GenerateUniqueCode(ct) :
            await EnsureUnique(req.CustomCode!, ct);

        var link = new Link
        {
            Code = code,
            TargetUrl = req.TargetUrl,
            ExpiresAt = req.ExpiresAt
        };

        await links.AddAsync(link, ct);
        await uow.SaveChangesAsync(ct);

        var shortUrl = $"{baseUrl().TrimEnd('/')}/{code}";
        return new LinkResponse(link.Id, link.Code, shortUrl, link.TargetUrl, link.CreatedAt, link.ExpiresAt);
    }

    public Task<Link?> GetByCodeAsync(string code, CancellationToken ct) =>
        links.GetByCodeAsync(code, ct);

    public async Task RecordClickAsync(
        Link link,
        string? referrer,
        string? userAgent,
        string? ip,
        IDictionary<string, string?> utm,
        CancellationToken ct)
    {
        static string? Get(IDictionary<string, string?> d, string k)
            => d.TryGetValue(k, out var v) ? v : null;

        var ev = new ClickEvent
        {
            LinkId = link.Id,
            Referrer = referrer,
            UserAgent = userAgent,
            Ip = ip,
            UtmSource = Get(utm, "utm_source"),
            UtmMedium = Get(utm, "utm_medium"),
            UtmCampaign = Get(utm, "utm_campaign")
        };

        await clicks.AddAsync(ev, ct);
        await uow.SaveChangesAsync(ct);
    }


    public async Task<StatsResponse> GetStatsAsync(Guid linkId, CancellationToken ct)
    {
        var link = await links.GetByIdAsync(linkId, ct);

        var query = clicks.QueryByLinkId(linkId);

        var total = query.LongCount();

        var byDay = query
            .AsEnumerable()
            .GroupBy(c => DateOnly.FromDateTime(c.OccurredAt))
            .Select(g => new ClicksByDay(g.Key, g.LongCount()))
            .OrderBy(x => x.Day)
            .ToList();

        var topRef = query
            .AsEnumerable()
            .GroupBy(c => c.Referrer ?? "(none)")
            .Select(g => new TopItem(g.Key, g.LongCount()))
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();

        var topSource = query
            .AsEnumerable()
            .GroupBy(c => c.UtmSource ?? "(none)")
            .Select(g => new TopItem(g.Key, g.LongCount()))
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();

        var topCampaign = query
            .AsEnumerable()
            .GroupBy(c => c.UtmCampaign ?? "(none)")
            .Select(g => new TopItem(g.Key, g.LongCount()))
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();


        return new StatsResponse(link.Id, link.Code, total, byDay, topRef, topSource, topCampaign);
    }

    private async Task<string> GenerateUniqueCode(CancellationToken ct)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        while (true)
        {
            var bytes = RandomNumberGenerator.GetBytes(6);
            var chars = new char[6];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = alphabet[bytes[i] % alphabet.Length];

            var code = new string(chars);
            if (!await links.ExistsByCodeAsync(code, ct))
                return code;
        }
    }

    private async Task<string> EnsureUnique(string custom, CancellationToken ct)
    {
        if (await links.ExistsByCodeAsync(custom, ct))
            throw new InvalidOperationException("Código já em uso.");
        return custom;
    }
}