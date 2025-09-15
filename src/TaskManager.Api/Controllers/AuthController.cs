using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    public record LoginRequest(string UserName);
    public record TokenResponse(string AccessToken, DateTime ExpiresAtUtc);

    [HttpPost("login")]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserName))
            return BadRequest("UserName is required");

        var secret = _config["Jwt:Secret"] ?? "super-secret-key-change-me-please-1234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, req.UserName),
        };

        var expires = DateTime.UtcNow.AddHours(8);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new TokenResponse(jwt, expires));
    }
}
