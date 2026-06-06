using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sales.Domain.Services;
using Sales.Infrastructure.Services;
using System.Text.Json;

namespace Sales.Infrastructure.BackgroundServices;

public class EmailQueueConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly SesEmailService _sesEmail;
    private readonly string _queueUrl;

    public EmailQueueConsumer(IAmazonSQS sqs, SesEmailService sesEmail, IConfiguration configuration)
    {
        _sqs = sqs;
        _sesEmail = sesEmail;
        _queueUrl = configuration["AWS:SQS:QueueUrl"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, stoppingToken);

            foreach (var msg in response.Messages)
            {
                var email = JsonSerializer.Deserialize<EmailMessage>(msg.Body);
                if (email is null) continue;

                await _sesEmail.SendConfirmationAsync(email, stoppingToken);
                await _sqs.DeleteMessageAsync(_queueUrl, msg.ReceiptHandle, stoppingToken);
            }
        }
    }
}
