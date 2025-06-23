using MediatR;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Domain.Contracts;

public class GetLastChatMessagesQuery : IRequest<List<ChatMessageDto>>
{
    public int FirstChatParticipantId { get; set; }
    public int SecondChatParticipantId { get; set; }
}