namespace Shared.Broker.Contracts;

public class CreateCommentContract
{
    public string Content { get; set; }
    public string Login { get; set; }
    public int ArticleId { get; set; }
}