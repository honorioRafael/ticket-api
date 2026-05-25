using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Customers.CreateCustomer;
using Sales.Application.Features.Customers.DeleteCustomer;
using Sales.Application.Features.Customers.GetAllCustomers;
using Sales.Application.Features.Customers.GetCustomer;
using Sales.Application.Features.Customers.UpdateCustomer;

namespace Sales.API.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;
    private readonly GetCustomerUseCase _getCustomerUseCase;
    private readonly GetAllCustomersUseCase _getAllCustomersUseCase;
    private readonly UpdateCustomerUseCase _updateCustomerUseCase;
    private readonly DeleteCustomerUseCase _deleteCustomerUseCase;

    public CustomersController(CreateCustomerUseCase createCustomerUseCase, GetCustomerUseCase getCustomerUseCase, GetAllCustomersUseCase getAllCustomersUseCase, UpdateCustomerUseCase updateCustomerUseCase, DeleteCustomerUseCase deleteCustomerUseCase)
    {
        _createCustomerUseCase = createCustomerUseCase;
        _getCustomerUseCase = getCustomerUseCase;
        _getAllCustomersUseCase = getAllCustomersUseCase;
        _updateCustomerUseCase = updateCustomerUseCase;
        _deleteCustomerUseCase = deleteCustomerUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _createCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var customer = await _getCustomerUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _getAllCustomersUseCase.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(id, request.Name, request.Email, request.Document);
        var customer = await _updateCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _deleteCustomerUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public record UpdateCustomerRequest(string Name, string Email, string Document);
