using Events.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Venues.DeleteVenue;

public class DeleteVenueUseCase
{
    private readonly IVenueRepository _venueRepository;

    public DeleteVenueUseCase(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task ExecuteAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
        if (venue == null)
            throw new DomainException("VENUE_NOT_FOUND", "Local não encontrado.");

        _venueRepository.Remove(venue);
        await _venueRepository.SaveChangesAsync(cancellationToken);
    }
}
