using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Orders.CreateOrder;
using Sales.Application.Features.Orders.DeleteOrder;
using Sales.Application.Features.Orders.GetAllOrders;
using Sales.Application.Features.Orders.GetOrder;
using Sales.Application.Features.Payments.ProcessPayment;

namespace Sales.API.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderUseCase _createOrderUseCase;
    private readonly GetOrderUseCase _getOrderUseCase;
    private readonly GetAllOrdersUseCase _getAllOrdersUseCase;
    private readonly DeleteOrderUseCase _deleteOrderUseCase;
    private readonly ProcessPaymentUseCase _processPaymentUseCase;

    public OrdersController(CreateOrderUseCase createOrderUseCase, GetOrderUseCase getOrderUseCase, GetAllOrdersUseCase getAllOrdersUseCase, DeleteOrderUseCase deleteOrderUseCase, ProcessPaymentUseCase processPaymentUseCase)
    {
        _createOrderUseCase = createOrderUseCase;
        _getOrderUseCase = getOrderUseCase;
        _getAllOrdersUseCase = getAllOrdersUseCase;
        _deleteOrderUseCase = deleteOrderUseCase;
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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllOrdersUseCase.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _deleteOrderUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/payment")]
    public async Task<IActionResult> ProcessPayment([FromRoute] Guid id, [FromBody] ProcessPaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id, request.Method);
        var payment = await _processPaymentUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(payment);
    }
}

public record ProcessPaymentRequest(string Method);
