using LinkShortener.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace LinkShortener.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var keycloak = _config.GetSection("Keycloak");
        var realm = keycloak["realm"];
        var clientId = keycloak["resource"];
        var clientSecret = keycloak["credentials:secret"];
        var authServer = keycloak["auth-server-url"]?.TrimEnd('/');

        var httpClient = _httpClientFactory.CreateClient();
        var tokenUrl = $"{authServer}/realms/{realm}/protocol/openid-connect/token";

        var body = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("username", request.Email),
                new KeyValuePair<string, string>("password", request.Password),
            });

        var response = await httpClient.PostAsync(tokenUrl, body);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, content);
        }

        var json = JsonDocument.Parse(content).RootElement;
        return Ok(new
        {
            access_token = json.GetProperty("access_token").GetString(),
            expires_in = json.GetProperty("expires_in").GetInt32(),
            refresh_token = json.GetProperty("refresh_token").GetString()
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var authHeader = HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
            return Unauthorized("Token não encontrado.");

        var token = authHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var id = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        var json = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(jwt.Payload));

        var roles = new List<string>();

        if (json.TryGetProperty("realm_access", out var realmAccess) &&
            realmAccess.TryGetProperty("roles", out var realmRoles))
        {
            foreach (var role in realmRoles.EnumerateArray())
                roles.Add(role.GetString()!);
        }

        if (json.TryGetProperty("resource_access", out var resourceAccess))
        {
            foreach (var resource in resourceAccess.EnumerateObject())
            {
                if (resource.Value.TryGetProperty("roles", out var clientRoles))
                {
                    foreach (var role in clientRoles.EnumerateArray())
                        roles.Add(role.GetString()!);
                }
            }
        }

        return Ok(new
        {
            Id = id,
            Username = username,
            Name = name,
            Email = email,
            Roles = roles.Distinct().ToList()
        });
    }
}

public record LoginRequest(string Email, string Password);