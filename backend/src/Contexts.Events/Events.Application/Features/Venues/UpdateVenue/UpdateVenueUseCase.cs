using Events.Application.DTOs;
using Events.Domain.Repositories;
using FluentValidation;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Venues.UpdateVenue;

public class UpdateVenueUseCase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<UpdateVenueCommand> _validator;

    public UpdateVenueUseCase(IVenueRepository venueRepository, IValidator<UpdateVenueCommand> validator)
    {
        _venueRepository = venueRepository;
        _validator = validator;
    }

    public async Task<VenueDto> ExecuteAsync(UpdateVenueCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = await _venueRepository.GetByIdAsync(command.Id, cancellationToken);
        if (venue == null)
            throw new DomainException(DomainErrorCode.NotFound, "Local não encontrado.");

        venue.Update(command.Name, command.Address, command.Capacity);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.Capacity);
    }
}
