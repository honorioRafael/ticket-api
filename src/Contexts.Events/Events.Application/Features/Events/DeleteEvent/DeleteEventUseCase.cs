using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.Features.Events.DeleteEvent;

public class DeleteEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventUseCase(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        _eventRepository.Remove(@event);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
