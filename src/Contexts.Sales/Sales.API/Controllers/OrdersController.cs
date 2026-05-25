using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands;
using Sales.Application.UseCases;

namespace Sales.API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderUseCase _createOrderUseCase;
    private readonly GetOrderUseCase _getOrderUseCase;
    private readonly ProcessPaymentUseCase _processPaymentUseCase;

    public OrdersController(
        CreateOrderUseCase createOrderUseCase,
        GetOrderUseCase getOrderUseCase,
        ProcessPaymentUseCase processPaymentUseCase)
    {
        _createOrderUseCase = createOrderUseCase;
        _getOrderUseCase = getOrderUseCase;
        _processPaymentUseCase = processPaymentUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _createOrderUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var order = await _getOrderUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPost("{id:guid}/payment")]
    public async Task<IActionResult> ProcessPayment(
        [FromRoute] Guid id,
        [FromBody] ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id, request.Method);
        var payment = await _processPaymentUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(payment);
    }
}

public record ProcessPaymentRequest(string Method);
