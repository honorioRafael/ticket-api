using Events.Application.DTOs;
using Events.Domain.Repositories;
using FluentValidation;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.CreateTicketType;

public class CreateTicketTypeUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<CreateTicketTypeCommand> _validator;

    public CreateTicketTypeUseCase(IEventRepository eventRepository, IVenueRepository venueRepository, IValidator<CreateTicketTypeCommand> validator)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _validator = validator;
    }
    public async Task<TicketTypeDto> ExecuteAsync(CreateTicketTypeCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var @event = await _eventRepository.GetByIdAsync(command.EventId, cancellationToken);
        if (@event == null)
            throw new DomainException(DomainErrorCode.NotFound, "Evento não encontrado.");

        var venue = await _venueRepository.GetByIdAsync(@event.VenueId, cancellationToken);
        if (venue == null)
            throw new DomainException(DomainErrorCode.NotFound, "Local não encontrado.");

        @event.AddTicketType(command.Name, command.Price, command.TotalQuantity, venue);
        var added = @event.TicketTypes.OrderByDescending(t => t.Id).First();
        await _eventRepository.AddTicketTypeAsync(added, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        return new TicketTypeDto(added.Id, added.EventId, added.Name, added.Price, added.TotalQuantity, added.AvailableQuantity);
    }
}
