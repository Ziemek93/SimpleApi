namespace MainApi.Models.Entities;

public class Tag
{
    public int TagId { get; set; }
    public string Title { get; set; }
    public virtual ICollection<Article>? Articles { get; set; }

}

