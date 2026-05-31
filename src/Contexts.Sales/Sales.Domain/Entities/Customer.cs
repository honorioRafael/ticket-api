using TicketApi.Common.Exceptions;

namespace Sales.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Document { get; private set; } = null!;

    private Customer() { }

    public Customer(string name, string email, string document)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("INVALID_EMAIL", "O email não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(document))
            throw new DomainException("INVALID_DOCUMENT", "O documento não pode ser vazio.");

        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Document = document;
    }

    public void Update(string name, string email, string document)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("INVALID_EMAIL", "O email não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(document))
            throw new DomainException("INVALID_DOCUMENT", "O documento não pode ser vazio.");

        Name = name;
        Email = email;
        Document = document;
    }
}
