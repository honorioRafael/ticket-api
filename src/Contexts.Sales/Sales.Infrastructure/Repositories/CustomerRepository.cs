using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly SalesDbContext _context;

    public CustomerRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Document == document, cancellationToken);
    }
}
