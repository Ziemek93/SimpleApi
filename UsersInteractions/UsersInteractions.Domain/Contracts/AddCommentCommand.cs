using MediatR;

namespace UsersInteractions.Domain.Contracts;

public class AddCommentCommand
    : IRequest
{
    public string Content { get; set; }
    public string Login { get; set; }
    public int ArticleId { get; set; }
}