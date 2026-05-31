using AutoMapper;
using Events.Application.DTOs;
using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.GetEvent;

public class GetEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public GetEventUseCase(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<EventDto> ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (@event == null)
            throw new DomainException("EVENT_NOT_FOUND", "Evento não encontrado.");

        return _mapper.Map<EventDto>(@event);
    }
}
