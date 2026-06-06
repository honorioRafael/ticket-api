namespace Events.API.Requests;

public record CreateTicketTypeRequest(string Name, decimal Price, int TotalQuantity);
