using AutoMapper;
using Events.Application.DTOs;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Features.Events.UpdateEvent;

public class UpdateEventUseCase
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateEventCommand> _validator;
    private readonly IMapper _mapper;

    public UpdateEventUseCase(IEventRepository eventRepository, IVenueRepository venueRepository, IUnitOfWork unitOfWork, IValidator<UpdateEventCommand> validator, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<EventDto> ExecuteAsync(UpdateEventCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var @event = await _eventRepository.GetByIdAsync(command.Id, cancellationToken);
        if (@event == null)
            throw new EventNotFoundException();

        var venue = await _venueRepository.GetByIdAsync(command.VenueId, cancellationToken);
        if (venue == null)
            throw new VenueNotFoundException();

        @event.Update(command.Name, command.StartsAt, command.EndsAt, command.VenueId);
        _eventRepository.Update(@event);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<EventDto>(@event);
    }
}
