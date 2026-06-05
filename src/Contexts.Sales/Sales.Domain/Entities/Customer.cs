using Sales.Domain.ValueObjects;
using TicketApi.Common.Exceptions;

namespace Sales.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Document Document { get; private set; } = null!;
    public Password Password { get; private set; } = null!;

    private Customer() { }

    public Customer(string name, string email, string document, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome não pode ser vazio.");

        Id = Guid.CreateVersion7();
        Name = name;
        Email = new Email(email);
        Document = new Document(document);
        Password = new Password(password);
    }

    public void Update(string name, string email, string document)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome não pode ser vazio.");

        Name = name;
        Email = new Email(email);
        Document = new Document(document);
    }

    public void ChangePassword(string newPassword)
    {
        Password = new Password(newPassword);
    }
}
