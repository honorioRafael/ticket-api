using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    void Update(Order order);
    void Remove(Order order);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
