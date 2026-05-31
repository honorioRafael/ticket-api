using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.PublishEvent;

public class PublishEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;

    public PublishEventUseCase(IEventRepository eventRepository, IVenueRepository venueRepository)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new DomainException("EVENT_NOT_FOUND", "Evento não encontrado.");

        var venue = await _venueRepository.GetByIdAsync(@event.VenueId, cancellationToken);
        if (venue == null)
            throw new DomainException("VENUE_NOT_FOUND", "Local não encontrado.");

        @event.Publish(venue);
        await _eventRepository.SaveChangesAsync(cancellationToken);
    }
}
