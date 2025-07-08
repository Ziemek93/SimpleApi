using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using UsersInteractions.Application.Abstractions;

namespace UsersInteractions.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            throw new SecurityTokenArgumentException();
        }
    }
}
