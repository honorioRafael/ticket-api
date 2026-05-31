using Events.Domain.ValueObjects;
using TicketApi.Common.Exceptions;

namespace Events.Domain.Entities;

public class Event
{
    private readonly List<TicketType> _ticketTypes = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTimeRange Period { get; private set; } = null!;
    public Guid VenueId { get; private set; }

    public IReadOnlyCollection<TicketType> TicketTypes => _ticketTypes.AsReadOnly();

    public bool IsActive(DateTime now)
    {
        return Period.Contains(now);
    }

    private Event() { }

    public Event(string name, DateTime startsAt, DateTime endsAt, Guid venueId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome do evento não pode ser vazio.");

        Id = Guid.CreateVersion7();
        Name = name;
        Period = new DateTimeRange(startsAt, endsAt);
        VenueId = venueId;
    }

    public void Update(string name, DateTime startsAt, DateTime endsAt, Guid venueId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome do evento não pode ser vazio.");

        Name = name;
        Period = new DateTimeRange(startsAt, endsAt);
        VenueId = venueId;
    }

    public void AddTicketType(string name, decimal price, int totalQuantity, Venue venue)
    {
        if (venue.Id != VenueId)
            throw new DomainException(DomainErrorCode.RuleViolation, "O local informado não coincide com o local do evento.");

        int currentTotalCapacity = _ticketTypes.Sum(t => t.TotalQuantity);
        if (currentTotalCapacity + totalQuantity > venue.Capacity)
            throw new DomainException(DomainErrorCode.RuleViolation, "A capacidade do local foi excedida.");

        var ticketType = new TicketType(Id, name, price, totalQuantity);
        _ticketTypes.Add(ticketType);
    }

    public void UpdateTicketType(Guid ticketTypeId, string name, decimal price, int totalQuantity, Venue venue)
    {
        var ticketType = _ticketTypes.FirstOrDefault(t => t.Id == ticketTypeId);
        if (ticketType == null)
            throw new DomainException(DomainErrorCode.NotFound, "Tipo de ingresso não encontrado neste evento.");

        int otherTicketTypesCapacity = _ticketTypes.Where(t => t.Id != ticketTypeId).Sum(t => t.TotalQuantity);
        if (otherTicketTypesCapacity + totalQuantity > venue.Capacity)
            throw new DomainException(DomainErrorCode.RuleViolation, "A capacidade do local foi excedida.");

        ticketType.Update(name, price, totalQuantity);
    }
}
