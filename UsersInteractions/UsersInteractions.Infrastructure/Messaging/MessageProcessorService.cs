using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using UsersInteractions.Application.Abstractions;

namespace UsersInteractions.Infrastructure.Messaging;

public class MessageProcessorService : IMessageProcessorService
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly ILogger<MessageProcessorService> _logger;

    public MessageProcessorService(IHubContext<MessageHub> hubContext, ILogger<MessageProcessorService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task ProcessNewMessage(string sender, string receiver, string messageContent, DateTime created, CancellationToken ct = default)
    {
        var notificationData = new
        {
            senderId = sender,
            receiverId = receiver,
            content = messageContent,
            date = created
        };

        // Send to the sender of the message (if connected)
        await _hubContext.Clients.User(sender).SendAsync("ReceiveNewMessage", notificationData, ct);

        // Send to the receiver of the message (if connected)
        await _hubContext.Clients.User(receiver).SendAsync("ReceiveNewMessage", notificationData, ct);
        
        _logger.LogInformation("SignalR notification sent to {sender} and {receiver}", sender, receiver);
    }
}