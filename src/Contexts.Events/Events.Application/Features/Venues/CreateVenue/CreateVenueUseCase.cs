using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Features.Venues.CreateVenue;

public class CreateVenueUseCase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateVenueCommand> _validator;

    public CreateVenueUseCase(IVenueRepository venueRepository, IUnitOfWork unitOfWork, IValidator<CreateVenueCommand> validator)
    {
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<VenueDto> ExecuteAsync(CreateVenueCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = new Venue(command.Name, command.Address, command.Capacity);
        await _venueRepository.AddAsync(venue, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
