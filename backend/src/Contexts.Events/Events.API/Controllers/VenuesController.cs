using Events.API.Requests;
using Events.Application.Features.Venues.CreateVenue;
using Events.Application.Features.Venues.DeleteVenue;
using Events.Application.Features.Venues.GetAllVenues;
using Events.Application.Features.Venues.GetVenue;
using Events.Application.Features.Venues.UpdateVenue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("venues")]
[Authorize]
public class VenuesController : ControllerBase
{
    private readonly CreateVenueUseCase _createVenueUseCase;
    private readonly GetVenueUseCase _getVenueUseCase;
    private readonly UpdateVenueUseCase _updateVenueUseCase;
    private readonly DeleteVenueUseCase _deleteVenueUseCase;
    private readonly GetAllVenuesUseCase _getAllVenuesUseCase;

    public VenuesController(CreateVenueUseCase createVenueUseCase, GetVenueUseCase getVenueUseCase, UpdateVenueUseCase updateVenueUseCase, DeleteVenueUseCase deleteVenueUseCase, GetAllVenuesUseCase getAllVenuesUseCase)
    {
        _createVenueUseCase = createVenueUseCase;
        _getVenueUseCase = getVenueUseCase;
        _updateVenueUseCase = updateVenueUseCase;
        _deleteVenueUseCase = deleteVenueUseCase;
        _getAllVenuesUseCase = getAllVenuesUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVenueCommand command, CancellationToken cancellationToken)
    {
        var venue = await _createVenueUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = venue.Id }, venue);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var venue = await _getVenueUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(venue);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllVenuesUseCase.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateVenueRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateVenueCommand(id, request.Name, request.Address, request.Capacity);
        var venue = await _updateVenueUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(venue);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _deleteVenueUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
