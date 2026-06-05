namespace Sales.Application.Features.Payments.PaymentWebhook;

public record PaymentWebhookCommand(Guid OrderId, Guid CustomerId, string Status, string Method);
