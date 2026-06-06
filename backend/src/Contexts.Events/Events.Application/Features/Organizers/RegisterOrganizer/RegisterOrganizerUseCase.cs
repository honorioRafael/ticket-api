using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using FluentValidation;
using TicketApi.Common.Auth;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Organizers.RegisterOrganizer;

public class RegisterOrganizerUseCase
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterOrganizerCommand> _validator;

    public RegisterOrganizerUseCase(IOrganizerRepository organizerRepository, IPasswordHasher passwordHasher, IValidator<RegisterOrganizerCommand> validator)
    {
        _organizerRepository = organizerRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<OrganizerDto> ExecuteAsync(RegisterOrganizerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _organizerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existing != null)
        {
            throw new DomainException(DomainErrorCode.ValidationError, "Já existe um organizador cadastrado com este e-mail.");
        }

        var hashedPassword = _passwordHasher.HashPassword(command.Password);
        var organizer = new Organizer(command.Name, command.Email, hashedPassword);

        await _organizerRepository.AddAsync(organizer, cancellationToken);
        await _organizerRepository.SaveChangesAsync(cancellationToken);

        return new OrganizerDto(organizer.Id, organizer.Name, organizer.Email);
    }
}
