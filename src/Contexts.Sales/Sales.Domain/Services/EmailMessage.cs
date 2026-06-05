namespace Sales.Domain.Services;

public record EmailMessage(string To, string CustomerName, Guid OrderId, decimal TotalAmount, int TicketCount);
