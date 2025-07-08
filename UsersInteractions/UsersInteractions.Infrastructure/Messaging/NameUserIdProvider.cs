using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace UsersInteractions.Infrastructure.Messaging;

public class NameUserIdProvider : IUserIdProvider
{
    public virtual string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}