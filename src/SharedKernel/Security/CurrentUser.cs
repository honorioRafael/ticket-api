using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SharedKernel.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var value = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? user?.FindFirst("sub")?.Value;
            
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value 
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public string? Role => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
