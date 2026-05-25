namespace Events.Application.Features.Organizers.RegisterOrganizer;

public record RegisterOrganizerCommand(string Name, string Email, string Password);
