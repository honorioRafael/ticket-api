using AutoMapper;
using Events.Application.DTOs;
using Events.Domain.Repositories;
using TicketApi.Common.Models;

namespace Events.Application.Features.Events.GetAllEvents;

public class GetAllEventsUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public GetAllEventsUseCase(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<EventDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _eventRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = _mapper.Map<List<EventDto>>(items);

        return new PaginatedList<EventDto>(dtos, page, pageSize, totalCount);
    }
}
