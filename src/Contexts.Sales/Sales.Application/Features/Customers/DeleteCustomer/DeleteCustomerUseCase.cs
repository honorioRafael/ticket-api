using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Customers.DeleteCustomer;

public class DeleteCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerUseCase(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new CustomerNotFoundException();

        _customerRepository.Remove(customer);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
