namespace Sales.Application.DTOs;

public record OrderItemDto(Guid Id, Guid TicketTypeId, decimal UnitPrice, int Quantity);
