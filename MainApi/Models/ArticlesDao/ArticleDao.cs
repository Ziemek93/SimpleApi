namespace MainApi.Models.ArticlesDao
{
    public class ArticleDao
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string? ArticleDescription { get; set; }
        public bool Visibility { get; set; }
        public List<string> Tags { get; set; }
    }
}
