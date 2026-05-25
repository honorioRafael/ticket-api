using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Customers.CreateCustomer;

public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerUseCase(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
    }
}
