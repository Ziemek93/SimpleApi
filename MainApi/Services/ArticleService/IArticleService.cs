using MainApi.Models.ArticlesDao;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;

namespace MainApi.Services.ArticleService
{
    public interface IArticleService
    {
        Task<ServiceResult<IEnumerable<ArticleDto>>> GetArticlesAsync(Func<Article, ArticleDto> mapper, CancellationToken token = default);
        Task<ServiceResult<ArticleDto>> GetArticleAsync(int id, Func<Article, ArticleDto> mapper, CancellationToken token = default);
        Task<ServiceResult<Article>> AddArticleAsync(Article article, CancellationToken token = default);
        Task<ServiceResult<bool>> EditArticleAsync(int id, ArticleDao article, CancellationToken token = default);
        Task<ServiceResult<bool>> DeleteArticleAsync(int id, CancellationToken token = default);
        Task<ServiceResult<bool>> CheckArticleAsync(int id, CancellationToken token = default);
    }
}