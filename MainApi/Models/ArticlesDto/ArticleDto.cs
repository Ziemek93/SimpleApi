namespace MainApi.Models.ArticlesDto
{
    public class ArticleDto
    {
        public string ArticleName { get; set; } = null!;
        public string? ArticleDescription { get; set; }
        public bool Visibility { get; set; }
        public List<String> Tags { get; set; }

        public virtual IEnumerable<ArticleCommentDto> Comments { get; set; }
    }
}
