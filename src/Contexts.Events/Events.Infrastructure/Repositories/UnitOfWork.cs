using System.Threading;
using System.Threading.Tasks;
using Events.Domain.Repositories;
using Events.Infrastructure.Contexts;

namespace Events.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventsDbContext _context;

    public UnitOfWork(EventsDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
