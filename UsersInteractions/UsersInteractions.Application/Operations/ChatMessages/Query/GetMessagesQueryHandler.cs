using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Contracts;
using UsersInteractions.Domain.Models;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Application.Operations.ChatMessages.Query;

public class GetMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, List<ChatMessageDto>>
{
    private readonly IApplicationContext _dbContext;

    public GetMessagesQueryHandler(IApplicationContext context)
    {
        _dbContext = context.CreateDbContext();
    }

    public async Task<List<ChatMessageDto>> Handle(GetChatMessagesQuery request,
        CancellationToken ct = default)
    {
        IQueryable<ChatMessage> dbData = _dbContext.ChatMessages
            .Where(x => (x.SenderId == request.FirstChatParticipantId &&
                         x.RecipientId == request.SecondChatParticipantId) ||
                        (x.RecipientId == request.FirstChatParticipantId &&
                         x.SenderId == request.SecondChatParticipantId
                         && (request.DateFrom == null || x.CreatedAt > request.DateFrom)
                         && (request.DateTo == null || x.CreatedAt < request.DateTo))
            )
            .OrderByDescending(x => x.CreatedAt);

        if (request.PaginationSize is not null)
        {
            if (request.PageNumber is not null)
            {
                dbData = dbData
                    .Skip((int)request.PaginationSize * (int)request.PageNumber)
                    .Take((int)request.PaginationSize);
            }
            dbData = dbData
                .Take((int)request.PaginationSize);
        }

        return await dbData.Select(x => new ChatMessageDto(x.SenderId, x.RecipientId, x.Content, x.CreatedAt)).ToListAsync(ct);
    }
}

public class GetMessagesValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetMessagesValidator()
    {
        RuleFor(x => x.FirstChatParticipantId)
            .NotNull();
        RuleFor(x => x.SecondChatParticipantId)
            .NotNull();
    }
}