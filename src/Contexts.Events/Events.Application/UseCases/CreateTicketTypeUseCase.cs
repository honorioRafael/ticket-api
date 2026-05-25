using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.UseCases;

public class CreateTicketTypeUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTicketTypeCommand> _validator;

    public CreateTicketTypeUseCase(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateTicketTypeCommand> validator)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<TicketTypeDto> ExecuteAsync(CreateTicketTypeCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var @event = await _eventRepository.GetByIdAsync(command.EventId, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        var venue = await _venueRepository.GetByIdAsync(@event.VenueId, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        @event.AddTicketType(command.Name, command.Price, command.TotalQuantity, venue);
        _eventRepository.Update(@event);
        await _unitOfWork.CommitAsync(cancellationToken);

        var added = @event.TicketTypes.OrderByDescending(t => t.Id).First();
        return new TicketTypeDto(added.Id, added.EventId, added.Name, added.Price, added.TotalQuantity, added.AvailableQuantity);
    }
}
