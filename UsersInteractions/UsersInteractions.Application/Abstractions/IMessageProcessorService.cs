namespace UsersInteractions.Application.Abstractions;

public interface IMessageProcessorService
{
    Task ProcessNewMessage(string sender, string receiver, string messageContent, DateTime created, CancellationToken ct = default);
}