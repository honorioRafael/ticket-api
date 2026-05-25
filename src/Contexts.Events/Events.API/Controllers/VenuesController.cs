using System;
using System.Threading;
using System.Threading.Tasks;
using Events.Application.Commands;
using Events.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("venues")]
public class VenuesController : ControllerBase
{
    private readonly CreateVenueUseCase _createVenueUseCase;
    private readonly GetVenueUseCase _getVenueUseCase;

    public VenuesController(CreateVenueUseCase createVenueUseCase, GetVenueUseCase getVenueUseCase)
    {
        _createVenueUseCase = createVenueUseCase;
        _getVenueUseCase = getVenueUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVenueCommand command, CancellationToken cancellationToken)
    {
        var venue = await _createVenueUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = venue.Id }, venue);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var venue = await _getVenueUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(venue);
    }
}
