using Events.Application.DTOs;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using FluentValidation;
using SharedKernel.Security;

namespace Events.Application.Features.Organizers.RegisterOrganizer;

public class RegisterOrganizerUseCase
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterOrganizerCommand> _validator;

    public RegisterOrganizerUseCase(
        IOrganizerRepository organizerRepository,
        IUnitOfWork unitOfWork,
        IValidator<RegisterOrganizerCommand> validator)
    {
        _organizerRepository = organizerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<OrganizerDto> ExecuteAsync(RegisterOrganizerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _organizerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existing != null)
        {
            throw new ArgumentException("O e-mail informado já está cadastrado.");
        }

        var passwordHash = PasswordHasher.HashPassword(command.Password);
        var organizer = new Organizer(command.Name, command.Email, passwordHash);

        await _organizerRepository.AddAsync(organizer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new OrganizerDto(organizer.Id, organizer.Name, organizer.Email);
    }
}
