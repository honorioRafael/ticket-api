using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Features.Events.CreateEvent;

public class CreateEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateEventCommand> _validator;

    public CreateEventUseCase(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateEventCommand> validator)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<EventDto> ExecuteAsync(CreateEventCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = await _venueRepository.GetByIdAsync(command.VenueId, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        var @event = new Event(command.Name, command.StartsAt, command.EndsAt, command.VenueId);
        await _eventRepository.AddAsync(@event, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return MapToDto(@event);
    }

    public static EventDto MapToDto(Event @event)
    {
        var ticketTypes = @event.TicketTypes.Select(t => new TicketTypeDto(
            t.Id, t.EventId, t.Name, t.Price, t.TotalQuantity, t.AvailableQuantity
        )).ToList();

        return new EventDto(
            @event.Id,
            @event.Name,
            @event.StartsAt,
            @event.EndsAt,
            @event.Status.ToString().ToLower(),
            @event.VenueId,
            ticketTypes
        );
    }
}
