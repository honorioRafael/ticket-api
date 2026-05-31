using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.DeleteEvent;

public class DeleteEventUseCase
{
    private readonly IEventRepository _eventRepository;

    public DeleteEventUseCase(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new DomainException(DomainErrorCode.NotFound, "Evento não encontrado.");

        _eventRepository.Remove(@event);
        await _eventRepository.SaveChangesAsync(cancellationToken);
    }
}
