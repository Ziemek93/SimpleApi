using FluentValidation;
using MassTransit;
using MediatR;
using Shared.Broker.Contracts;
using UsersInteractions.Domain.Contracts;

namespace UsersInteractions.Infrastructure.Consumers;

public class MessageReceivedEventHandler : IConsumer<CreateChatMessageContract>
{
    private readonly IMediator _mediator;

    public MessageReceivedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateChatMessageContract> eventBody)
    {
        var message = eventBody.Message;
        var request = new SendMessageCommand
        {
            SenderId = message.SenderId,
            RecipientId = message.RecipientId,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };

        await _mediator.Send(request);
    }
}

public class ChatMessageReceivedValidator : AbstractValidator<CreateChatMessageContract>
{
    public ChatMessageReceivedValidator()
    {
        RuleFor(x=>x.Content).NotNull().NotEmpty().MaximumLength(1000);
    }
}