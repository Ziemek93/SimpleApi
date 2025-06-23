using MediatR;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Domain.Contracts;

public class GetCommentsQuery
    : IRequest<List<CommentDto>>
{
    public int ArticleId { get; set; }
}