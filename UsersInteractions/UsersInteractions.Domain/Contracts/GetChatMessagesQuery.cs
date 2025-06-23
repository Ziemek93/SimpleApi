using MediatR;
using UsersInteractions.Domain.Models.Dtos;

namespace UsersInteractions.Domain.Contracts;

public class GetChatMessagesQuery : IRequest<List<ChatMessageDto>>
{
    public int FirstChatParticipantId { get; set; }
    public int SecondChatParticipantId { get; set; }
    public int? PaginationSize { get; set; }
    public int? PageNumber { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    
}