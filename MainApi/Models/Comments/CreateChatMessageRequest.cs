using System.Security.Claims;
using FastEndpoints;

namespace MainApi.Models.Comments;

public class CreateChatMessageRequest
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public string UserId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}