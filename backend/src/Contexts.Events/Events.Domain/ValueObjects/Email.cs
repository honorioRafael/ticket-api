using System.Text.RegularExpressions;
using TicketApi.Common.Exceptions;

namespace Events.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(DomainErrorCode.ValidationError, "O email não pode ser vazio.");

        string sanitized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(sanitized))
            throw new DomainException(DomainErrorCode.ValidationError, "O email informado é inválido.");

        Value = sanitized;
    }

    public static implicit operator string(Email email) => email?.Value!;
}
