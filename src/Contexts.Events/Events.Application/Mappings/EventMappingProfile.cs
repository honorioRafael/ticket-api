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
            .ForCtorParam("status", opt => opt.MapFrom(src => src.Status.ToString().ToLower()));
    }
}
