using Events.Application.DTOs;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Features.Venues.UpdateVenue;

public class UpdateVenueUseCase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateVenueCommand> _validator;

    public UpdateVenueUseCase(
        IVenueRepository venueRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateVenueCommand> validator)
    {
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<VenueDto> ExecuteAsync(UpdateVenueCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = await _venueRepository.GetByIdAsync(command.Id, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        venue.Update(command.Name, command.Address, command.Capacity);
        _venueRepository.Update(venue);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
