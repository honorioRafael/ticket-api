using Events.Domain.Enums;
using TicketApi.Common.Exceptions;

namespace Events.Domain.Entities;

public class Event
{
    private readonly List<TicketType> _ticketTypes = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public EventStatus Status { get; private set; }
    public Guid VenueId { get; private set; }

    public IReadOnlyCollection<TicketType> TicketTypes => _ticketTypes.AsReadOnly();

    public bool IsActive(DateTime now)
    {
        return Status == EventStatus.Published && now < StartsAt;
    }

    private Event() { }

    public Event(string name, DateTime startsAt, DateTime endsAt, Guid venueId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do evento não pode ser vazio.");
        if (endsAt <= startsAt)
            throw new DomainException("INVALID_EVENT_DATES", "A data de término do evento deve ser posterior à data de início.");

        Id = Guid.CreateVersion7();
        Name = name;
        StartsAt = startsAt;
        EndsAt = endsAt;
        VenueId = venueId;
        Status = EventStatus.Draft;
    }

    public void Update(string name, DateTime startsAt, DateTime endsAt, Guid venueId)
    {
        if (Status != EventStatus.Draft)
            throw new DomainException("INVALID_STATE_TRANSITION", "Não é possível atualizar detalhes do evento a menos que ele esteja em rascunho.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do evento não pode ser vazio.");
        if (endsAt <= startsAt)
            throw new DomainException("INVALID_EVENT_DATES", "A data de término do evento deve ser posterior à data de início.");

        Name = name;
        StartsAt = startsAt;
        EndsAt = endsAt;
        VenueId = venueId;
    }

    public void AddTicketType(string name, decimal price, int totalQuantity, Venue venue)
    {
        if (Status != EventStatus.Draft)
            throw new DomainException("TICKET_TYPE_READ_ONLY", "Não é possível adicionar tipos de ingresso a um evento que não esteja em rascunho.");

        if (venue.Id != VenueId)
            throw new DomainException("VENUE_MISMATCH", "O local informado não coincide com o local do evento.");

        int currentTotalCapacity = _ticketTypes.Sum(t => t.TotalQuantity);
        if (currentTotalCapacity + totalQuantity > venue.Capacity)
            throw new DomainException("CAPACITY_EXCEEDED", "A capacidade do local foi excedida.");

        var ticketType = new TicketType(Id, name, price, totalQuantity);
        _ticketTypes.Add(ticketType);
    }

    public void UpdateTicketType(Guid ticketTypeId, string name, decimal price, int totalQuantity, Venue venue)
    {
        if (Status != EventStatus.Draft)
            throw new DomainException("TICKET_TYPE_READ_ONLY", "Não é possível atualizar tipos de ingresso de um evento que não esteja em rascunho.");

        var ticketType = _ticketTypes.FirstOrDefault(t => t.Id == ticketTypeId);
        if (ticketType == null)
            throw new DomainException("TICKET_TYPE_NOT_FOUND", "Tipo de ingresso não encontrado neste evento.");

        int otherTicketTypesCapacity = _ticketTypes.Where(t => t.Id != ticketTypeId).Sum(t => t.TotalQuantity);
        if (otherTicketTypesCapacity + totalQuantity > venue.Capacity)
            throw new DomainException("CAPACITY_EXCEEDED", "A capacidade do local foi excedida.");

        ticketType.Update(name, price, totalQuantity, Status);
    }

    public void Publish(Venue venue)
    {
        if (Status != EventStatus.Draft)
            throw new DomainException("INVALID_STATE_TRANSITION", $"Não é possível publicar o evento a partir do status {Status}.");

        if (!_ticketTypes.Any(t => t.TotalQuantity > 0))
            throw new DomainException("EVENT_WITHOUT_TICKETS", "Não é possível publicar um evento sem pelo menos um tipo de ingresso com quantidade maior que zero.");

        int currentTotalCapacity = _ticketTypes.Sum(t => t.TotalQuantity);
        if (currentTotalCapacity > venue.Capacity)
            throw new DomainException("CAPACITY_EXCEEDED", "A capacidade total dos ingressos excede a capacidade do local.");

        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Published)
            throw new DomainException("INVALID_STATE_TRANSITION", $"Não é possível cancelar o evento a partir do status {Status}.");

        Status = EventStatus.Cancelled;
    }

    public void Finish()
    {
        if (Status != EventStatus.Published)
            throw new DomainException("INVALID_STATE_TRANSITION", $"Não é possível finalizar o evento a partir do status {Status}.");

        Status = EventStatus.Finished;
    }
}
