using FluentValidation;
using MediatR;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Contracts;
using UsersInteractions.Domain.Models;

namespace UsersInteractions.Application.Operations.Comments.Command;



public class AddCommentCommandHandler
    : IRequestHandler<AddCommentCommand>
{
    private readonly IApplicationContext _dbContext;

    public AddCommentCommandHandler(IApplicationContext context)
    {
        _dbContext = context.CreateDbContext();
    }
    
    public async Task Handle(AddCommentCommand request, 
        CancellationToken ct = default)
    {
        var comment = new Comment
        {
            Content = request.Content,
            Login = request.Login,
            ArticleId = request.ArticleId
        };
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync(ct);
    }
}

public class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.ArticleId)
            .NotNull();
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(500);
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(50);
    }
}