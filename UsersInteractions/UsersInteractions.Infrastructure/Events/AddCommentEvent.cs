namespace UsersInteractions.Infrastructure.Events;

public class AddCommentEvent
{
    public string Content { get; set; }
    public string Login { get; set; }
    public int ArticleId { get; set; }
}