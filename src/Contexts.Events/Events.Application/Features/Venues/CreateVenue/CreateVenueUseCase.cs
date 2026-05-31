using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Features.Venues.CreateVenue;

public class CreateVenueUseCase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<CreateVenueCommand> _validator;

    public CreateVenueUseCase(IVenueRepository venueRepository, IValidator<CreateVenueCommand> validator)
    {
        _venueRepository = venueRepository;
        _validator = validator;
    }

    public async Task<VenueDto> ExecuteAsync(CreateVenueCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = new Venue(command.Name, command.Address, command.Capacity);
        await _venueRepository.AddAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
