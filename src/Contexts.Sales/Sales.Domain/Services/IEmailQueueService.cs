namespace Sales.Domain.Services;

public record EmailMessage(string To, string CustomerName, Guid OrderId, decimal TotalAmount, int TicketCount);

public interface IEmailQueueService
{
    Task PublishAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
