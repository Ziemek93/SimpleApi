namespace MainApi.Models.Entities;

public class Article
{
    public int ArticleId { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool Visibility { get; set; }
    public User User { get; set; }
    public Category Category { get; set; }
    public virtual ICollection<Tag> Tags { get; set; }
}
