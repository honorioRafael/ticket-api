using Events.Application.Features.Organizers.LoginOrganizer;
using Events.Application.Features.Organizers.RegisterOrganizer;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterOrganizerUseCase _registerOrganizerUseCase;
    private readonly LoginOrganizerUseCase _loginOrganizerUseCase;

    public AuthController(
        RegisterOrganizerUseCase registerOrganizerUseCase,
        LoginOrganizerUseCase loginOrganizerUseCase)
    {
        _registerOrganizerUseCase = registerOrganizerUseCase;
        _loginOrganizerUseCase = loginOrganizerUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterOrganizerCommand command, CancellationToken cancellationToken)
    {
        var result = await _registerOrganizerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginOrganizerCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginOrganizerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(result);
    }
}
