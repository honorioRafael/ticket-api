using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    void Update(Customer customer);
    void Remove(Customer customer);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
