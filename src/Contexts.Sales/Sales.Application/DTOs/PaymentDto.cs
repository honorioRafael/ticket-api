namespace Sales.Application.DTOs;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string Method,
    string Status,
    decimal Amount,
    DateTime? PaidAt
);
