using System;
using FluentValidation;

namespace Events.Application.Commands;

public record CreateVenueCommand(string Name, string Address, int Capacity);

public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("O endereço é obrigatório.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("A capacidade deve ser maior que zero.");
    }
}

public record CreateEventCommand(string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.StartsAt).NotEmpty().WithMessage("A data de início é obrigatória.");
        RuleFor(x => x.EndsAt).NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThan(x => x.StartsAt).WithMessage("A data de fim deve ser posterior à data de início.");
        RuleFor(x => x.VenueId).NotEmpty().WithMessage("O ID do local é obrigatório.");
    }
}

public record CreateTicketTypeCommand(Guid EventId, string Name, decimal Price, int TotalQuantity);

public class CreateTicketTypeCommandValidator : AbstractValidator<CreateTicketTypeCommand>
{
    public CreateTicketTypeCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty().WithMessage("O ID do evento é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("O preço deve ser maior ou igual a zero.");
        RuleFor(x => x.TotalQuantity).GreaterThan(0).WithMessage("A quantidade total deve ser maior que zero.");
    }
}
