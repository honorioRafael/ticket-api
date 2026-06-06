using Events.Application.Features.Organizers.LoginOrganizer;
using Events.Application.Features.Organizers.RegisterOrganizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("organizers")]
public class OrganizersController : ControllerBase
{
    private readonly RegisterOrganizerUseCase _registerOrganizerUseCase;
    private readonly LoginOrganizerUseCase _loginOrganizerUseCase;

    public OrganizersController(
        RegisterOrganizerUseCase registerOrganizerUseCase,
        LoginOrganizerUseCase loginOrganizerUseCase)
    {
        _registerOrganizerUseCase = registerOrganizerUseCase;
        _loginOrganizerUseCase = loginOrganizerUseCase;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterOrganizerCommand command, CancellationToken cancellationToken)
    {
        var result = await _registerOrganizerUseCase.ExecuteAsync(command, cancellationToken);
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginOrganizerCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginOrganizerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(result);
    }
}
