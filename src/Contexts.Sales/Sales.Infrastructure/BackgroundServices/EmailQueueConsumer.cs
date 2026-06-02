using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sales.Domain.Services;
using Sales.Infrastructure.Services;

namespace Sales.Infrastructure.BackgroundServices;

public class EmailQueueConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly SesEmailService _sesEmail;
    private readonly string _queueUrl;
    private readonly ILogger<EmailQueueConsumer> _logger;

    public EmailQueueConsumer(IAmazonSQS sqs, SesEmailService sesEmail, IConfiguration configuration, ILogger<EmailQueueConsumer> logger)
    {
        _sqs = sqs;
        _sesEmail = sesEmail;
        _queueUrl = configuration["AWS:SQS:QueueUrl"]!;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailQueueConsumer iniciado. Aguardando mensagens...");

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
                try
                {
                    var email = JsonSerializer.Deserialize<EmailMessage>(msg.Body);
                    if (email is null) continue;

                    await _sesEmail.SendConfirmationAsync(email, stoppingToken);

                    _logger.LogInformation("E-mail enviado para {To} | Pedido: {OrderId}", email.To, email.OrderId);

                    await _sqs.DeleteMessageAsync(_queueUrl, msg.ReceiptHandle, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila: {MessageId}", msg.MessageId);
                }
            }
        }
    }
}
