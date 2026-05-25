using System;
using System.Threading;
using System.Threading.Tasks;
using Events.Application.Commands;
using Events.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly CreateEventUseCase _createEventUseCase;
    private readonly GetEventUseCase _getEventUseCase;
    private readonly CreateTicketTypeUseCase _createTicketTypeUseCase;
    private readonly PublishEventUseCase _publishEventUseCase;
    private readonly CancelEventUseCase _cancelEventUseCase;

    public EventsController(
        CreateEventUseCase createEventUseCase,
        GetEventUseCase getEventUseCase,
        CreateTicketTypeUseCase createTicketTypeUseCase,
        PublishEventUseCase publishEventUseCase,
        CancelEventUseCase cancelEventUseCase)
    {
        _createEventUseCase = createEventUseCase;
        _getEventUseCase = getEventUseCase;
        _createTicketTypeUseCase = createTicketTypeUseCase;
        _publishEventUseCase = publishEventUseCase;
        _cancelEventUseCase = cancelEventUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventCommand command, CancellationToken cancellationToken)
    {
        var @event = await _createEventUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = @event.Id }, @event);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var @event = await _getEventUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(@event);
    }

    [HttpPost("{eventId:guid}/ticket-types")]
    public async Task<IActionResult> CreateTicketType(
        [FromRoute] Guid eventId,
        [FromBody] CreateTicketTypeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTicketTypeCommand(eventId, request.Name, request.Price, request.TotalQuantity);
        var ticketType = await _createTicketTypeUseCase.ExecuteAsync(command, cancellationToken);
        var location = Url.Action(nameof(GetById), "Events", new { id = eventId }) ?? $"/events/{eventId}";
        return Created(location, ticketType);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _publishEventUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _cancelEventUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public record CreateTicketTypeRequest(string Name, decimal Price, int TotalQuantity);
