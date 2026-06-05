namespace Sales.Application.Features.Customers.CreateCustomer;

public record CreateCustomerCommand(string Name, string Email, string Document, string Password);
