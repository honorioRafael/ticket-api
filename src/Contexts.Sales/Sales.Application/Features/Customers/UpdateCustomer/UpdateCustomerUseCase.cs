using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<UpdateCustomerCommand> _validator;

    public UpdateCustomerUseCase(ICustomerRepository customerRepository, IValidator<UpdateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<CustomerDto> ExecuteAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken);
        if (customer == null)
            throw new DomainException("CUSTOMER_NOT_FOUND", "Cliente não encontrado.");

        customer.Update(command.Name, command.Email, command.Document);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
    }
}
