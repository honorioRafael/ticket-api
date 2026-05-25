namespace Sales.Application.Features.Customers.UpdateCustomer;

public record UpdateCustomerCommand(Guid Id, string Name, string Email, string Document);
