using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateCustomerCommand> _validator;

    public UpdateCustomerUseCase(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IValidator<UpdateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CustomerDto> ExecuteAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.Id, cancellationToken);
        if (customer == null)
            throw new CustomerNotFoundException();

        customer.Update(command.Name, command.Email, command.Document);
        _customerRepository.Update(customer);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
    }
}
