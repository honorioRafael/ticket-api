using AutoMapper;
using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using FluentValidation;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Events.CreateEvent;

public class CreateEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<CreateEventCommand> _validator;
    private readonly IMapper _mapper;

    public CreateEventUseCase(IEventRepository eventRepository, IVenueRepository venueRepository, IValidator<CreateEventCommand> validator, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<EventDto> ExecuteAsync(CreateEventCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var venue = await _venueRepository.GetByIdAsync(command.VenueId, cancellationToken);
        if (venue == null)
            throw new DomainException(DomainErrorCode.NotFound, "Local não encontrado.");

        var @event = new Event(command.Name, command.StartsAt, command.EndsAt, command.VenueId);
        await _eventRepository.AddAsync(@event, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(@event);
    }
}
