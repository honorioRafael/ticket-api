using System;
using System.Collections.Generic;

namespace Sales.Application.DTOs;

public record CustomerDto(Guid Id, string Name, string Email, string Document);

public record OrderItemDto(Guid Id, Guid TicketTypeId, decimal UnitPrice, int Quantity);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    DateTime PlacedAt,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<OrderItemDto> Items
);

public record TicketDto(Guid Id, Guid OrderItemId, string Code, string Status);

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string Method,
    string Status,
    decimal Amount,
    DateTime? PaidAt
);
