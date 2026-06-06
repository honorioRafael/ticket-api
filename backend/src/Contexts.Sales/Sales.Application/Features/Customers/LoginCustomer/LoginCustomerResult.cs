using Sales.Application.DTOs;

namespace Sales.Application.Features.Customers.LoginCustomer;

public record LoginCustomerResult(string Token, CustomerDto Customer);
