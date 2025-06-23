using FluentValidation;
using MassTransit;
using MediatR;
using UsersInteractions.Domain.Contracts;
// using MassTransit.Mediator;
using UsersInteractions.Infrastructure.Events;

namespace UsersInteractions.Infrastructure.Consumers;

public class CommentReceivedEventHandler : IConsumer<AddCommentEvent>
{
    private readonly IMediator _mediator;

    public CommentReceivedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    // public CommentReceivedEventHandler()
    // { }

    public async Task Consume(ConsumeContext<AddCommentEvent> eventBody)
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

public class CommentReceivedValidator : AbstractValidator<AddCommentEvent>
{
    public CommentReceivedValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty().WithMessage("Id cannot be empty");
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Login).NotEmpty();
    }
}