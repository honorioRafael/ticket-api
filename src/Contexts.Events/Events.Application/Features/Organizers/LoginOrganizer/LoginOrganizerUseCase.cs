using Events.Domain.Repositories;
using FluentValidation;
using SharedKernel.Security;

namespace Events.Application.Features.Organizers.LoginOrganizer;

public class LoginOrganizerUseCase
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginOrganizerCommand> _validator;

    public LoginOrganizerUseCase(
        IOrganizerRepository organizerRepository,
        ITokenService tokenService,
        IValidator<LoginOrganizerCommand> validator)
    {
        _organizerRepository = organizerRepository;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<LoginResultDto> ExecuteAsync(LoginOrganizerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var organizer = await _organizerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (organizer == null || !PasswordHasher.VerifyPassword(organizer.PasswordHash, command.Password))
        {
            throw new ArgumentException("E-mail ou senha incorretos.");
        }

        var token = _tokenService.GenerateToken(organizer.Id, organizer.Email, "Organizer");

        return new LoginResultDto(token, organizer.Id, organizer.Name, organizer.Email);
    }
}
