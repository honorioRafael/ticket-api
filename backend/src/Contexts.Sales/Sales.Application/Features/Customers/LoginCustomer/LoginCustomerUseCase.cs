using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Auth;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Customers.LoginCustomer;

public class LoginCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<LoginCustomerCommand> _validator;

    public LoginCustomerUseCase(ICustomerRepository customerRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, IValidator<LoginCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
    }

    public async Task<LoginCustomerResult> ExecuteAsync(LoginCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (customer == null)
            throw new DomainException(DomainErrorCode.ValidationError, "E-mail ou senha incorretos.");

        var isPasswordValid = _passwordHasher.VerifyPassword(command.Password, customer.Password);
        if (!isPasswordValid)
            throw new DomainException(DomainErrorCode.ValidationError, "E-mail ou senha incorretos.");

        var token = _tokenGenerator.GenerateToken(customer.Id.ToString(), customer.Email.Value, "Customer", customer.Name);

        var customerDto = new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
        return new LoginCustomerResult(token, customerDto);
    }
}
