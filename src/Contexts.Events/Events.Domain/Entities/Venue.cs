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
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("O endereço não pode ser vazio.", nameof(address));
        if (capacity <= 0)
            throw new ArgumentException("A capacidade deve ser maior que zero.", nameof(capacity));

        Id = Guid.CreateVersion7();
        Name = name;
        Address = address;
        Capacity = capacity;
    }

    public void Update(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("O endereço não pode ser vazio.", nameof(address));
        if (capacity <= 0)
            throw new ArgumentException("A capacidade deve ser maior que zero.", nameof(capacity));

        Name = name;
        Address = address;
        Capacity = capacity;
    }
}
