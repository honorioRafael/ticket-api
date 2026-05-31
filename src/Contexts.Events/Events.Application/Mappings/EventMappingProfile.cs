using AutoMapper;
using Events.Application.DTOs;
using Events.Domain.Entities;

namespace Events.Application.Mappings;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<TicketType, TicketTypeDto>();
        CreateMap<Event, EventDto>()
            .ForCtorParam(nameof(EventDto.StartsAt), opt => opt.MapFrom(src => src.Period.Start))
            .ForCtorParam(nameof(EventDto.EndsAt), opt => opt.MapFrom(src => src.Period.End));
    }
}
