using Events.Application.DTOs;
using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Venues.GetVenue;

public class GetVenueUseCase
{
    private readonly IVenueRepository _venueRepository;

    public GetVenueUseCase(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<VenueDto> ExecuteAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
        if (venue == null)
            throw new DomainException(DomainErrorCode.NotFound, "Local não encontrado.");

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
