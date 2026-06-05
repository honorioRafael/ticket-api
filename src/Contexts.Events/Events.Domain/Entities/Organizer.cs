using Events.Domain.ValueObjects;
using TicketApi.Common.Exceptions;

namespace Events.Domain.Entities;

public class Organizer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;

    private Organizer() { }

    public Organizer(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome não pode ser vazio.");

        Id = Guid.CreateVersion7();
        Name = name;
        Email = new Email(email);
        Password = new Password(password);
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrorCode.ValidationError, "O nome não pode ser vazio.");

        Name = name;
        Email = new Email(email);
    }

    public void ChangePassword(string newPassword)
    {
        Password = new Password(newPassword);
    }
}
