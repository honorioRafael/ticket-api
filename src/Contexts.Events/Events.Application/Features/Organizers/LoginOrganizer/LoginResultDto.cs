namespace Events.Application.Features.Organizers.LoginOrganizer;

public record LoginResultDto(string Token, Guid Id, string Name, string Email);
