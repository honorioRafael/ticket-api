using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Customers.CreateCustomer;

public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerUseCase(ICustomerRepository customerRepository, IValidator<CreateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<CustomerDto> ExecuteAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _customerRepository.GetByDocumentAsync(command.Document, cancellationToken);
        if (existing != null)
        {
            return new CustomerDto(existing.Id, existing.Name, existing.Email, existing.Document);
        }

        var customer = new Customer(command.Name, command.Email, command.Document);
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
    }
}
