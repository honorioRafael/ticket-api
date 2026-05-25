using FluentValidation;
using Sales.Domain.Repositories;
using SharedKernel.Security;

namespace Sales.Application.Features.Customers.LoginCustomer;

public class LoginCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginCustomerCommand> _validator;

    public LoginCustomerUseCase(
        ICustomerRepository customerRepository, 
        ITokenService tokenService, 
        IValidator<LoginCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<LoginResultDto> ExecuteAsync(LoginCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (customer == null || !PasswordHasher.VerifyPassword(customer.PasswordHash, command.Password))
        {
            throw new ArgumentException("E-mail ou senha incorretos.");
        }

        var token = _tokenService.GenerateToken(customer.Id, customer.Email, "Customer");

        return new LoginResultDto(token, customer.Id, customer.Name, customer.Email);
    }
}
