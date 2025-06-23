using System.ComponentModel.DataAnnotations;

namespace UsersInteractions.Domain.Models;

public class Comment
{
    [Key]
    public int CommentId { get; set; }

    public string Content { get; set; }
    public string Login { get; set; }
    public int ArticleId { get; set; }
}
