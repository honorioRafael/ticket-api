namespace Sales.Application.Commands;

public record CreateCustomerCommand(string Name, string Email, string Document);
