using Microsoft.AspNetCore.Mvc;
using Sales.Application.Features.Customers.LoginCustomer;

namespace Sales.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly LoginCustomerUseCase _loginCustomerUseCase;

    public AuthController(LoginCustomerUseCase loginCustomerUseCase)
    {
        _loginCustomerUseCase = loginCustomerUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCustomerCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginCustomerUseCase.ExecuteAsync(command, cancellationToken);
        return Ok(result);
    }
}
