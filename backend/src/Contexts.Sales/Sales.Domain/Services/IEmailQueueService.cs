namespace Sales.Domain.Services;

public interface IEmailQueueService
{
    Task PublishAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
