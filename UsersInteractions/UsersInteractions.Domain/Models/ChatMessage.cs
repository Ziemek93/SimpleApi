using System.ComponentModel.DataAnnotations;

namespace UsersInteractions.Domain.Models;

public class ChatMessage
{
    [Key]
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
