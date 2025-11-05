using LinkShortener.Application.Contracts;
using LinkShortener.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LinkShortener.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LinksController(ILinkService service, IHttpContextAccessor accessor) : ControllerBase
{
    private readonly ILinkService _service = service;

    [HttpGet("stats/{id:guid}")]
    public async Task<IActionResult> GetStats([FromRoute] Guid id, CancellationToken ct)
    {
        try
        {
            var stats = await _service.GetStatsAsync(id, ct);
            return Ok(stats);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableRateLimiting("create-link")]
    public async Task<IActionResult> Create([FromBody] CreateLinkRequest req, CancellationToken ct)
    {
        if (!Uri.TryCreate(req.TargetUrl, UriKind.Absolute, out _))
            return BadRequest("URL inválida.");

        string BaseUrl() => $"{Request.Scheme}://{Request.Host}";
        var created = await _service.CreateAsync(req, BaseUrl, ct);
        return CreatedAtAction(nameof(GetStats), new { id = created.Id }, created);
    }
}