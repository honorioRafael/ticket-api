using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly SalesDbContext _context;

    public TicketRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddRangeAsync(tickets, cancellationToken);
    }

    public async Task<Ticket?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets.SingleOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
    }
}
