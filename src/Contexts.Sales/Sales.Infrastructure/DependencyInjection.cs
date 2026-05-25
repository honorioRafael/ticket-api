using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;
using Sales.Infrastructure.Repositories;

namespace Sales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseNpgsql(connectionString, o => 
                o.MigrationsHistoryTable("__EFMigrationsHistory", "sales")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
