using LinkShortener.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtService jwtService) : ControllerBase
{
    private readonly IJwtService _service = jwtService;

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Email == "teste@email.com" && request.Password == "1234")
        {
            var token = _service.GenerateToken("1", request.Email, "admin");
            return Ok(new { Token = token });
        }

        return Unauthorized("Credenciais inválidas.");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = new
        {
            Id = User.FindFirst("sub")?.Value,
            Email = User.FindFirst("email")?.Value,
            Role = User.FindFirst("role")?.Value
        };

        return Ok(user);
    }
}


public record LoginRequest(string Email, string Password);