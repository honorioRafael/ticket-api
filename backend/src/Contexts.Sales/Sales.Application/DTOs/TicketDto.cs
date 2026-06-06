namespace Sales.Application.DTOs;

public record TicketDto(Guid Id, Guid OrderItemId, string Code, string Status);
