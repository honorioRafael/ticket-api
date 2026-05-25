using System;
using System.Threading;
using System.Threading.Tasks;
using Events.Application.DTOs;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.UseCases;

public class GetEventUseCase
{
    private readonly IEventRepository _eventRepository;

    public GetEventUseCase(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto> ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        return CreateEventUseCase.MapToDto(@event);
    }
}
