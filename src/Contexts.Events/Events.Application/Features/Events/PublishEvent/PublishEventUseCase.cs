using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.Features.Events.PublishEvent;

public class PublishEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishEventUseCase(IEventRepository eventRepository, IVenueRepository venueRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        var venue = await _venueRepository.GetByIdAsync(@event.VenueId, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        @event.Publish(venue);
        _eventRepository.Update(@event);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
