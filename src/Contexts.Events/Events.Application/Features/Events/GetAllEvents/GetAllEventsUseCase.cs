using Events.Application.DTOs;
using Events.Application.Features.Events.CreateEvent;
using Events.Domain.Repositories;
using SharedKernel.Models;

namespace Events.Application.Features.Events.GetAllEvents;

public class GetAllEventsUseCase
{
    private readonly IEventRepository _eventRepository;

    public GetAllEventsUseCase(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PaginatedList<EventDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _eventRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = items.Select(CreateEventUseCase.MapToDto).ToList();

        return new PaginatedList<EventDto>(dtos, page, pageSize, totalCount);
    }
}
