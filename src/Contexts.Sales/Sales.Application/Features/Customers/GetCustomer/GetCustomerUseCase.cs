using Sales.Application.DTOs;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Customers.GetCustomer;

public class GetCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new CustomerNotFoundException();

        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Document);
    }
}
