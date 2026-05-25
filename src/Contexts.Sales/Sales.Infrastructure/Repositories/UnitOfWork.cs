using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SalesDbContext _context;

    public UnitOfWork(SalesDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Sales.Domain.Exceptions.InsufficientStockException("Concurrency conflict: ticket stock is no longer available.");
        }
    }
}
