namespace Sales.Application.Features.Customers.LoginCustomer;

public record LoginResultDto(string Token, Guid Id, string Name, string Email);
