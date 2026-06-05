using Events.Application.DTOs;
using Events.Domain.Repositories;
using FluentValidation;
using TicketApi.Common.Auth;
using TicketApi.Common.Exceptions;

namespace Events.Application.Features.Organizers.LoginOrganizer;

public class LoginOrganizerUseCase
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<LoginOrganizerCommand> _validator;

    public LoginOrganizerUseCase(IOrganizerRepository organizerRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator, IValidator<LoginOrganizerCommand> validator)
    {
        _organizerRepository = organizerRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
    }

    public async Task<LoginOrganizerResult> ExecuteAsync(LoginOrganizerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var organizer = await _organizerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (organizer == null)
            throw new DomainException(DomainErrorCode.ValidationError, "E-mail ou senha incorretos.");

        var isPasswordValid = _passwordHasher.VerifyPassword(command.Password, organizer.Password);
        if (!isPasswordValid)
            throw new DomainException(DomainErrorCode.ValidationError, "E-mail ou senha incorretos.");

        var token = _tokenGenerator.GenerateToken(organizer.Id.ToString(), organizer.Email, "Organizer", organizer.Name);

        var organizerDto = new OrganizerDto(organizer.Id, organizer.Name, organizer.Email);
        return new LoginOrganizerResult(token, organizerDto);
    }
}
