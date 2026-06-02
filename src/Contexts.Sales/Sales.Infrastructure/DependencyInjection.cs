using Amazon.SimpleEmail;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Domain.Repositories;
using Sales.Domain.Services;
using Sales.Infrastructure.BackgroundServices;
using Sales.Infrastructure.Contexts;
using Sales.Infrastructure.Repositories;
using Sales.Infrastructure.Services;

namespace Sales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseNpgsql(connectionString, o =>
                o.MigrationsHistoryTable("__EFMigrationsHistory")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        // AWS
        var region = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]!);
        var accessKey = configuration["AWS:AccessKeyId"]!;
        var secretKey = configuration["AWS:SecretAccessKey"]!;

        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(accessKey, secretKey, new AmazonSQSConfig { RegionEndpoint = region }));
        services.AddSingleton<IAmazonSimpleEmailService>(_ => new AmazonSimpleEmailServiceClient(accessKey, secretKey, new AmazonSimpleEmailServiceConfig { RegionEndpoint = region }));

        services.AddScoped<IEmailQueueService, SqsEmailQueueService>();
        services.AddSingleton<SesEmailService>();
        services.AddHostedService<EmailQueueConsumer>();

        return services;
    }
}
