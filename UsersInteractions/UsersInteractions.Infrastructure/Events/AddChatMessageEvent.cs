namespace UsersInteractions.Infrastructure.Events;

public class AddChatMessageEvent
{
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}