using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.CancelEvent;

public class CancelEventUseCase
{
    private readonly IEventRepository _eventRepository;

    public CancelEventUseCase(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new DomainException("EVENT_NOT_FOUND", "Evento não encontrado.");

        @event.Cancel();
        await _eventRepository.SaveChangesAsync(cancellationToken);
    }
}
