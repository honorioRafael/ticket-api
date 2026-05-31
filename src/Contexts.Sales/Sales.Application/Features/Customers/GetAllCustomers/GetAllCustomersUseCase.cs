using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Models;

namespace Sales.Application.Features.Customers.GetAllCustomers;

public class GetAllCustomersUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PaginatedList<CustomerDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _customerRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = items.Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.Document)).ToList();

        return new PaginatedList<CustomerDto>(dtos, page, pageSize, totalCount);
    }
}
