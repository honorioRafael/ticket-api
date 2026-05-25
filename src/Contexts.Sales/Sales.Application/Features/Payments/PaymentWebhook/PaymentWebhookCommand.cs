namespace Sales.Application.Features.Payments.PaymentWebhook;

public record PaymentWebhookCommand(Guid OrderId, string Status, string Method);
