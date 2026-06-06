using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Sales.Domain.Services;

namespace Sales.Infrastructure.Services;

public class SesEmailService
{
    private readonly IAmazonSimpleEmailService _ses;
    private readonly string _fromEmail;

    public SesEmailService(IAmazonSimpleEmailService ses, IConfiguration configuration)
    {
        _ses = ses;
        _fromEmail = configuration["AWS:SES:FromEmail"]
            ?? throw new InvalidOperationException("A configuração 'AWS:SES:FromEmail' não foi encontrada.");
    }

    public async Task SendConfirmationAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var body = $"""
            <h2>Confirmação de Compra</h2>
            <p>Olá, <strong>{message.CustomerName}</strong>!</p>
            <p>Seu pagamento foi confirmado com sucesso.</p>
            <ul>
                <li><strong>Pedido:</strong> {message.OrderId}</li>
                <li><strong>Total:</strong> R$ {message.TotalAmount:N2}</li>
                <li><strong>Ingressos:</strong> {message.TicketCount}</li>
            </ul>
            <p>Obrigado pela compra!</p>
            """;

        var request = new SendEmailRequest
        {
            Source = _fromEmail,
            Destination = new Destination { ToAddresses = [message.To] },
            Message = new Message
            {
                Subject = new Content($"Confirmação do Pedido {message.OrderId}"),
                Body = new Body
                {
                    Html = new Content(body)
                }
            }
        };

        await _ses.SendEmailAsync(request, cancellationToken);
    }
}
