using System;
using System.Linq;
using TicketApi.Common.Exceptions;

namespace Sales.Domain.ValueObjects;

public record Document
{
    public string Value { get; }

    public Document(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(DomainErrorCode.ValidationError, "O documento não pode ser vazio.");

        string cleaned = Clean(value);

        if (!IsValid(cleaned))
            throw new DomainException(DomainErrorCode.ValidationError, "O documento informado é inválido.");

        Value = cleaned;
    }

    private static string Clean(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    private static bool IsValid(string value)
    {
        if (value.Length != 11 && value.Length != 14)
            return false;

        if (value.Distinct().Count() == 1)
            return false;

        if (value.Length == 11)
            return IsValidCpf(value);

        return true;
    }

    private static bool IsValidCpf(string cpf)
    {
        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }

    public static implicit operator string(Document doc) => doc?.Value!;
}
