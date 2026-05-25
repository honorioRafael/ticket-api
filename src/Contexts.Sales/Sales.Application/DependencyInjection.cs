using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.UseCases;

namespace Sales.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CreateCustomerUseCase>();
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<ProcessPaymentUseCase>();
        services.AddScoped<PaymentWebhookUseCase>();
        services.AddScoped<ValidateTicketUseCase>();
        services.AddScoped<GetOrderUseCase>();

        return services;
    }
}
