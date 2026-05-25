using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.UseCases;

namespace Sales.API.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController : ControllerBase
{
    private readonly ValidateTicketUseCase _validateTicketUseCase;

    public TicketsController(ValidateTicketUseCase validateTicketUseCase)
    {
        _validateTicketUseCase = validateTicketUseCase;
    }

    [HttpPost("{code}/validate")]
    public async Task<IActionResult> Validate([FromRoute] string code, CancellationToken cancellationToken)
    {
        var ticket = await _validateTicketUseCase.ExecuteAsync(code, cancellationToken);
        return Ok(ticket);
    }
}
