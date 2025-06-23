using MainApi.Models.ArticlesDao;
using MainApi.Models.Entities;

namespace MainApi.Repositories.Articles
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetArticlesAsync(CancellationToken token = default);
        Task<Article?> GetArticleAsync(int id, CancellationToken token = default);
        Task<int> AddArticleAsync(Article article, CancellationToken token = default);
        Task<bool> CategoryExistAsync(int id, CancellationToken token = default);
        Task<bool> ArticleExistAsync(Article article, CancellationToken token = default);
        Task<bool> EditArticleAsync(int id, ArticleDao req, CancellationToken token = default);
        Task<bool> DeleteArticleAsync(int id, CancellationToken token = default);
    }
}
