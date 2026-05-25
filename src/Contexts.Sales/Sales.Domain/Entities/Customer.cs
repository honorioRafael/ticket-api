namespace Sales.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Document { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    private Customer() { }

    public Customer(string name, string email, string document, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email não pode ser vazio.", nameof(email));
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("O documento não pode ser vazio.", nameof(document));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("A senha não pode ser vazia.", nameof(passwordHash));

        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Document = document;
        PasswordHash = passwordHash;
    }

    public void Update(string name, string email, string document)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email não pode ser vazio.", nameof(email));
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("O documento não pode ser vazio.", nameof(document));

        Name = name;
        Email = email;
        Document = document;
    }
}
