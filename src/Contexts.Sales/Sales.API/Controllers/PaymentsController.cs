using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands;
using Sales.Application.UseCases;

namespace Sales.API.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentWebhookUseCase _paymentWebhookUseCase;

    public PaymentsController(PaymentWebhookUseCase paymentWebhookUseCase)
    {
        _paymentWebhookUseCase = paymentWebhookUseCase;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PaymentWebhookCommand command, CancellationToken cancellationToken)
    {
        await _paymentWebhookUseCase.ExecuteAsync(command, cancellationToken);
        return NoContent();
    }
}
