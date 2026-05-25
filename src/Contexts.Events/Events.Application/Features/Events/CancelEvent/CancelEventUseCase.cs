using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.Features.Events.CancelEvent;

public class CancelEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelEventUseCase(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        @event.Cancel();
        _eventRepository.Update(@event);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
