namespace MainApi.Models.Comments;

public class CreateChatMessageRequest
{
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}