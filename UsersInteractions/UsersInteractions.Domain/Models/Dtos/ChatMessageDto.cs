namespace UsersInteractions.Domain.Models.Dtos;

public record ChatMessageDto(
    int SenderId,
    int RecipientId,
    string Content,
    DateTime CreatedAt
);