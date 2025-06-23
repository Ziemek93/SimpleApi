using FastEndpoints;
using MainApi.Models.Entities;

namespace MainApi.Models.ArticlesDto
{
    public class ArticleDtoExpanded
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; } = null!;
        public string? ArticleDescription { get; set; }
        public Category Category { get; set; }
        public bool Visibility { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
    internal sealed class Validator : Validator<ArticleDtoExpanded>
    {
        public Validator()
        {

        }
    }
}
