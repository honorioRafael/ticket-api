using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Sales.Domain.Services;

namespace Sales.Infrastructure.Services;

public class SqsEmailQueueService : IEmailQueueService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;

    public SqsEmailQueueService(IAmazonSQS sqs, IConfiguration configuration)
    {
        _sqs = sqs;
        _queueUrl = configuration["AWS:SQS:QueueUrl"]
            ?? throw new InvalidOperationException("A configuração 'AWS:SQS:QueueUrl' não foi encontrada.");
    }

    public async Task PublishAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        };

        await _sqs.SendMessageAsync(request, cancellationToken);
    }
}
