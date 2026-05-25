using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands;
using Sales.Application.UseCases;

namespace Sales.API.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;

    public CustomersController(CreateCustomerUseCase createCustomerUseCase)
    {
        _createCustomerUseCase = createCustomerUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _createCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return Created($"/customers/{customer.Id}", customer);
    }
}
