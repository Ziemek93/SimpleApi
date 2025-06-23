namespace MainApi.Models.Entities;

public class User
{
    public int UserId { get; set; }
   
    public string UserName { get; set; }
    public bool Deleted { get; set; }

    public virtual ICollection<Article> Articles { get; set; }
    public virtual ICollection<Category> Categories { get; set; }

}