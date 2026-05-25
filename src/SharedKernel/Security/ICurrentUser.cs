namespace SharedKernel.Security;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
