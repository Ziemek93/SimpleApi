using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace UsersInteractions.Infrastructure.Messaging;

[Authorize]
public class MessageHub : Hub
{
    private readonly ILogger<MessageHub> _logger;

    public MessageHub(ILogger<MessageHub> logger)
    {
        _logger = logger;
    }
    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("--> Connection with: {Context.UserIdentifier} (ConnectionId: {Context.ConnectionId})", 
            Context.UserIdentifier, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("--> Connection ended: {Context.UserIdentifier} (ConnectionId: {Context.ConnectionId})", 
            Context.UserIdentifier, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}