using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Customers.CreateCustomer;
using Sales.Application.Features.Customers.DeleteCustomer;
using Sales.Application.Features.Customers.GetAllCustomers;
using Sales.Application.Features.Customers.GetCustomer;
using Sales.Application.Features.Customers.UpdateCustomer;
using SharedKernel.Security;

namespace Sales.API.Controllers;

[ApiController]
[Route("customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;
    private readonly GetCustomerUseCase _getCustomerUseCase;
    private readonly GetAllCustomersUseCase _getAllCustomersUseCase;
    private readonly UpdateCustomerUseCase _updateCustomerUseCase;
    private readonly DeleteCustomerUseCase _deleteCustomerUseCase;
    private readonly ICurrentUser _currentUser;

    public CustomersController(
        CreateCustomerUseCase createCustomerUseCase, 
        GetCustomerUseCase getCustomerUseCase, 
        GetAllCustomersUseCase getAllCustomersUseCase, 
        UpdateCustomerUseCase updateCustomerUseCase, 
        DeleteCustomerUseCase deleteCustomerUseCase,
        ICurrentUser currentUser)
    {
        _createCustomerUseCase = createCustomerUseCase;
        _getCustomerUseCase = getCustomerUseCase;
        _getAllCustomersUseCase = getAllCustomersUseCase;
        _updateCustomerUseCase = updateCustomerUseCase;
        _deleteCustomerUseCase = deleteCustomerUseCase;
        _currentUser = currentUser;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _createCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.Id != id)
        {
            return Forbid("Você não tem permissão para acessar os dados de outro cliente.");
        }

        var customer = await _getCustomerUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        // Apenas para demonstrar restrição de acesso administrativo
        return Forbid("Apenas administradores podem listar todos os clientes.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id != id)
        {
            return Forbid("Você não tem permissão para atualizar os dados de outro cliente.");
        }

        var command = new UpdateCustomerCommand(id, request.Name, request.Email, request.Document);
        var customer = await _updateCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.Id != id)
        {
            return Forbid("Você não tem permissão para excluir a conta de outro cliente.");
        }

        await _deleteCustomerUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

public record UpdateCustomerRequest(string Name, string Email, string Document);
