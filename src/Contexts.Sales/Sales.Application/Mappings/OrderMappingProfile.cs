using AutoMapper;
using Sales.Application.DTOs;
using Sales.Domain.Entities;

namespace Sales.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>()
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString().ToLower()))
            .ForCtorParam("Items", opt => opt.MapFrom(src => src.OrderItems));
    }
}
