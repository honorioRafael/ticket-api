using TicketApi.Common.Exceptions;

namespace Events.Domain.ValueObjects;

public record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(DomainErrorCode.ValidationError, "A senha não pode ser vazia.");

        Value = value;
    }

    public static implicit operator string(Password password) => password?.Value!;
}
