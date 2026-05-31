using TicketApi.Common.Exceptions;

namespace Events.Domain.Entities;

public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public int Capacity { get; private set; }

    private Venue() { }

    public Venue(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do local não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("INVALID_ADDRESS", "O endereço do local não pode ser vazio.");
        if (capacity <= 0)
            throw new DomainException("INVALID_CAPACITY", "A capacidade do local deve ser maior que zero.");

        Id = Guid.CreateVersion7();
        Name = name;
        Address = address;
        Capacity = capacity;
    }

    public void Update(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do local não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("INVALID_ADDRESS", "O endereço do local não pode ser vazio.");
        if (capacity <= 0)
            throw new DomainException("INVALID_CAPACITY", "A capacidade do local deve ser maior que zero.");

        Name = name;
        Address = address;
        Capacity = capacity;
    }
}
