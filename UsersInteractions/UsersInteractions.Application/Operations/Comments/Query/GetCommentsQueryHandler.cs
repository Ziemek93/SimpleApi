using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Contracts;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Application.Operations.Comments.Query;

public class GetCommentsQueryHandler
    : IRequestHandler<GetCommentsQuery, List<CommentDto>>
{
    private readonly IApplicationContext _dbContext;

    public GetCommentsQueryHandler(IApplicationContext context)
    {
        _dbContext = context.CreateDbContext();
    }
    
    public async Task<List<CommentDto>> Handle(GetCommentsQuery request, 
        CancellationToken ct = default)
    {
        var response = await _dbContext.Comments
            .Where(x => x.ArticleId == request.ArticleId)
            .Select(x=>new CommentDto(x.CommentId,x.Content, x.Login, x.ArticleId))
            .ToListAsync(ct);

        return response;
    }
}

public class GetCommentsValidator : AbstractValidator<GetCommentsQuery>
{
    public GetCommentsValidator()
    {
        RuleFor(x=>x.ArticleId).NotNull();
    }
}