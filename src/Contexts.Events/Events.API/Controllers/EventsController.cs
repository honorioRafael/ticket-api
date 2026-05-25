using Events.Application.Features.Events.CancelEvent;
using Events.Application.Features.Events.CreateEvent;
using Events.Application.Features.Events.CreateTicketType;
using Events.Application.Features.Events.DeleteEvent;
using Events.Application.Features.Events.GetAllEvents;
using Events.Application.Features.Events.GetEvent;
using Events.Application.Features.Events.PublishEvent;
using Events.Application.Features.Events.UpdateEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers;

[ApiController]
[Route("events")]
[Authorize(Roles = "Organizer")]
public class EventsController : ControllerBase
{
    private readonly CreateEventUseCase _createEventUseCase;
    private readonly GetEventUseCase _getEventUseCase;
    private readonly UpdateEventUseCase _updateEventUseCase;
    private readonly DeleteEventUseCase _deleteEventUseCase;
    private readonly GetAllEventsUseCase _getAllEventsUseCase;
    private readonly CreateTicketTypeUseCase _createTicketTypeUseCase;
    private readonly PublishEventUseCase _publishEventUseCase;
    private readonly CancelEventUseCase _cancelEventUseCase;

    public EventsController(CreateEventUseCase createEventUseCase, GetEventUseCase getEventUseCase, UpdateEventUseCase updateEventUseCase, DeleteEventUseCase deleteEventUseCase, GetAllEventsUseCase getAllEventsUseCase, CreateTicketTypeUseCase createTicketTypeUseCase, PublishEventUseCase publishEventUseCase, CancelEventUseCase cancelEventUseCase)
    {
        _createEventUseCase = createEventUseCase;
        _getEventUseCase = getEventUseCase;
        _updateEventUseCase = updateEventUseCase;
        _deleteEventUseCase = deleteEventUseCase;
        _getAllEventsUseCase = getAllEventsUseCase;
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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllEventsUseCase.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand(id, request.Name, request.StartsAt, request.EndsAt, request.VenueId);
        var @event = await _updateEventUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(@event);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _deleteEventUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{eventId:guid}/ticket-types")]
    public async Task<IActionResult> CreateTicketType([FromRoute] Guid eventId, [FromBody] CreateTicketTypeRequest request, CancellationToken cancellationToken)
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

public record UpdateEventRequest(string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);
