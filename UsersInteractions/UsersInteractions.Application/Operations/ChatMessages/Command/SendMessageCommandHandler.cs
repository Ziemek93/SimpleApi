using System.Text.Json;
using FluentValidation;
using MediatR;
using UsersInteractions.Application.Abstractions;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Models;
using UsersInteractions.Domain.Contracts;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Application.Operations.ChatMessages.Command;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly IApplicationContext _dbContext;
    private readonly ICacheService _cache;
    private const string CacheChatMessagesKeyPattern = "Chat_SenderId:{0};RecipientId:{1};";//"Date{2};";
    public SendMessageCommandHandler(IApplicationContext context, ICacheService cache)
    {
        _dbContext = context.CreateDbContext();
        _cache = cache;
    }
    
    public async Task Handle(SendMessageCommand request, 
        CancellationToken ct = default)
    {
        int smallerId = Math.Min(request.SenderId, request.RecipientId);
        int largerId = Math.Max(request.SenderId, request.RecipientId);
        
        string cacheKey = string.Format(
            CacheChatMessagesKeyPattern,
            smallerId,
            largerId);
            // request.CreatedAt.ToString("yyyy-MM-dd"));

        var message = new ChatMessage
        {
            SenderId = request.SenderId,
            RecipientId = request.RecipientId,
            Content = request.Content,
            CreatedAt = request.CreatedAt
        };
        
        _dbContext.ChatMessages.Add(message);

        var serializeMessage = JsonSerializer.Serialize(
            new ChatMessageDto(request.SenderId, request.RecipientId, request.Content, request.CreatedAt)
            );
        
        // await _cache.GetAsync<List<ChatMessage>>(cacheKey);
        // _cache.SetAsync(cacheKey, serializeMessage)
        await _cache.ListRightPushAsync(cacheKey, serializeMessage);
        await _cache.ListTrimAsync(cacheKey, 100, 1);
        
        await _dbContext.SaveChangesAsync(ct);
    }
}

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.SenderId)
            .NotNull();
        RuleFor(x => x.RecipientId)
            .NotNull();
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(500);
    }
}