using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Customers.DeleteCustomer;

public class DeleteCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new DomainException(DomainErrorCode.NotFound, "Cliente não encontrado.");

        _customerRepository.Remove(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);
    }
}
