namespace TicketApi.Common.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, string role, string name);
}
