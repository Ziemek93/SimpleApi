using MainApi.Models.CategoriesDto;
using MainApi.Models.Entities;

namespace MainApi.Models.ArticlesDto
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public CategoryDto Category { get; set; }
        public bool Visibility { get; set; }
        public List<String> Tags { get; set; }

        public virtual IEnumerable<ArticleCommentDto> Comments { get; set; }
    }
}
