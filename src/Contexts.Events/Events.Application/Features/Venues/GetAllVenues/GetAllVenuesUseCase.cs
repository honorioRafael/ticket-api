using Events.Application.DTOs;
using Events.Domain.Repositories;
using SharedKernel.Models;

namespace Events.Application.Features.Venues.GetAllVenues;

public class GetAllVenuesUseCase
{
    private readonly IVenueRepository _venueRepository;

    public GetAllVenuesUseCase(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<PaginatedList<VenueDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _venueRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = items.Select(v => new VenueDto(v.Id, v.Name, v.Address, v.Capacity)).ToList();

        return new PaginatedList<VenueDto>(dtos, page, pageSize, totalCount);
    }
}
