namespace Events.Domain.Entities;

public class Organizer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    private Organizer() { }

    public Organizer(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email não pode ser vazio.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("A senha não pode ser vazia.", nameof(passwordHash));

        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
}
