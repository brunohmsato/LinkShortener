using LinkShortener.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RedirectController(ILinkService service) : ControllerBase
{
    private readonly ILinkService _service = service;

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToTarget([FromRoute] string code, CancellationToken ct)
    {
        var link = await _service.GetByCodeAsync(code, ct);

        if (link is null || (link.ExpiresAt is not null && link.ExpiresAt < DateTime.UtcNow) || !link.IsActive)
            return NotFound("Link não encontrado ou expirado.");

        var headers = Request.Headers;
        string? referrer = headers.Referer.FirstOrDefault() ?? headers["Referer"].FirstOrDefault();
        string? userAgent = headers.UserAgent.ToString();
        string? ip = headers.TryGetValue("X-Forwarded-For", out var fof)
            ? fof.FirstOrDefault()?.Split(',')[0].Trim()
            : HttpContext.Connection.RemoteIpAddress?.ToString();

        var utm = new Dictionary<string, string?>
        {
            ["utm_source"] = Request.Query["utm_source"],
            ["utm_medium"] = Request.Query["utm_medium"],
            ["utm_campaign"] = Request.Query["utm_campaign"]
        };

        await _service.RecordClickAsync(link, referrer, userAgent, ip, utm, ct);

        return Redirect(link.TargetUrl);
    }
}