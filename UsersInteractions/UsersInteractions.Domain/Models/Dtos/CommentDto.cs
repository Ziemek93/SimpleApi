namespace UsersInteractions.Domain.Models.Dtos;

public record CommentDto(int Id, string Content, string Login, int ArticleId);
