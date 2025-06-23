using System.Text.Json;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UsersInteractions.Application.Abstractions;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Contracts;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Application.Operations.ChatMessages.Query;

public class GetLastMessagesQueryHandler : IRequestHandler<GetLastChatMessagesQuery, List<ChatMessageDto>>
{
    private readonly IApplicationContext _dbContext;
    private readonly ICacheService _cache;
    private const string CacheChatMessagesKeyPattern = "Chat_SenderId:{0};RecipientId:{1};"; //"Date{2};";

    public GetLastMessagesQueryHandler(IApplicationContext context, ICacheService cache)
    {
        _dbContext = context.CreateDbContext();
        _cache = cache;
    }

    public async Task<List<ChatMessageDto>> Handle(GetLastChatMessagesQuery request,
        CancellationToken ct = default)
    {
        var smallerId = Math.Min(request.FirstChatParticipantId, request.SecondChatParticipantId);
        var largerId = Math.Max(request.FirstChatParticipantId, request.SecondChatParticipantId);

        var cacheKey = string.Format(
            CacheChatMessagesKeyPattern,
            smallerId,
            largerId);

        var cacheData = await _cache.ListRangeAsync(cacheKey);
        if (cacheData.Any())
            return cacheData
                .Select(x => JsonSerializer.Deserialize<ChatMessageDto>(x!))
                .Where(x => x != null)
                .OfType<ChatMessageDto>().ToList();

        var dbData = await _dbContext.ChatMessages
            .Where(x => (x.SenderId == smallerId && x.RecipientId == largerId) ||
                        (x.RecipientId == smallerId && x.SenderId == largerId))
            .Select(x => new ChatMessageDto(x.SenderId, x.RecipientId, x.Content, x.CreatedAt))
            .OrderByDescending(x => x.CreatedAt)
            .Take(100)
            .ToListAsync(ct);

        var redisValues = dbData
            .OrderBy(x=>x.CreatedAt)
            .Select(msg => (RedisValue)JsonSerializer.Serialize(msg))
            .ToArray();

        await _cache.ListRightPushAsync(cacheKey, redisValues);

        return dbData;
    }
}

public class GetLastMessagesValidator : AbstractValidator<GetLastChatMessagesQuery>
{
    public GetLastMessagesValidator()
    {
        RuleFor(x => x.FirstChatParticipantId)
            .NotNull();
        RuleFor(x => x.SecondChatParticipantId)
            .NotNull();
    }
}