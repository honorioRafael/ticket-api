using System;
using System.Threading;
using System.Threading.Tasks;
using Events.Application.DTOs;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.UseCases;

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
            throw new VenueNotFoundException();

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
