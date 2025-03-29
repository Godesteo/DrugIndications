using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrugIndications.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User credentials)
    {
        var user = _authService.ValidateUser(credentials.Username, credentials.PasswordHash);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _authService.GenerateToken(user);
        return Ok(new { token });
    }
}
