namespace Shared.Broker.Contracts;

public class CreateChatMessageContract
{
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}