namespace Sales.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    DateTime PlacedAt,
    decimal TotalAmount,
    string Status,
    IReadOnlyCollection<OrderItemDto> Items
);
