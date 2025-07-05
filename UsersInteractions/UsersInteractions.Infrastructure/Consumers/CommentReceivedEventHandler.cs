using FluentValidation;
using MassTransit;
using MediatR;
using Shared.Broker.Contracts;
using UsersInteractions.Domain.Contracts;

namespace UsersInteractions.Infrastructure.Consumers;

public class CommentReceivedEventHandler : IConsumer<CreateCommentContract>
{
    private readonly IMediator _mediator;

    public CommentReceivedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateCommentContract> eventBody)
    {
        var message = eventBody.Message;
        var request = new AddCommentCommand
        {
            Content = message.Content,
            Login = message.Login,
            ArticleId = message.ArticleId
        };

        await _mediator.Send(request);
    }
}

public class CommentReceivedValidator : AbstractValidator<CreateCommentContract>
{
    public CommentReceivedValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty().WithMessage("Id cannot be empty");
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Login).NotEmpty();
    }
}