using Events.Domain.Exceptions;
using Events.Domain.Repositories;

namespace Events.Application.Features.Venues.DeleteVenue;

public class DeleteVenueUseCase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVenueUseCase(IVenueRepository venueRepository, IUnitOfWork unitOfWork)
    {
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        _venueRepository.Remove(venue);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
