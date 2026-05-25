using System;
using System.Threading;
using System.Threading.Tasks;
using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
}
