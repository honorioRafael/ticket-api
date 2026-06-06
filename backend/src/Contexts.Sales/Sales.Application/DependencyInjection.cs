using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Features.Customers.CreateCustomer;
using Sales.Application.Features.Customers.DeleteCustomer;
using Sales.Application.Features.Customers.GetAllCustomers;
using Sales.Application.Features.Customers.GetCustomer;
using Sales.Application.Features.Customers.LoginCustomer;
using Sales.Application.Features.Customers.UpdateCustomer;
using Sales.Application.Features.Orders.CreateOrder;
using Sales.Application.Features.Orders.DeleteOrder;
using Sales.Application.Features.Orders.GetAllOrders;
using Sales.Application.Features.Orders.GetOrder;
using Sales.Application.Features.Payments.PaymentWebhook;
using Sales.Application.Features.Payments.ProcessPayment;
using Sales.Application.Features.Tickets.ValidateTicket;

namespace Sales.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

        services.AddScoped<CreateCustomerUseCase>();
        services.AddScoped<GetCustomerUseCase>();
        services.AddScoped<GetAllCustomersUseCase>();
        services.AddScoped<UpdateCustomerUseCase>();
        services.AddScoped<DeleteCustomerUseCase>();
        services.AddScoped<LoginCustomerUseCase>();

        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<GetOrderUseCase>();
        services.AddScoped<GetAllOrdersUseCase>();
        services.AddScoped<DeleteOrderUseCase>();

        services.AddScoped<ProcessPaymentUseCase>();
        services.AddScoped<PaymentWebhookUseCase>();

        services.AddScoped<ValidateTicketUseCase>();

        return services;
    }
}
