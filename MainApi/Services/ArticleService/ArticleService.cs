using MainApi.Models;
using MainApi.Models.ArticlesDao;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;
using MainApi.Models.Enums;
using MainApi.Repositories.Articles;

namespace MainApi.Services.ArticleService
{

    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articlerepository;

        public ArticleService(IArticleRepository articlerepository)
        {
            _articlerepository = articlerepository;
        }

        public async Task<ServiceResult<Article>> AddArticleAsync(Article article, CancellationToken token = default)
        {
            var categoryExists = await _articlerepository.CategoryExistAsync(article.CategoryId, token);
            if (!categoryExists)
            {
                return ServiceResult<Article>.Failure("Category don't exist");
            }
            else
            {
                var result = await _articlerepository.AddArticleAsync(article, token);
                if (result == 0)
                {
                    return ServiceResult<Article>.Failure("Article can't be added.");
                    
                }
                return ServiceResult<Article>.Success(article);
            }

        }

        public async Task<ServiceResult<bool>> EditArticleAsync(int id, ArticleDao article, CancellationToken token = default)
        {
            var result = await _articlerepository.EditArticleAsync(id, article);

            if (!result)
            {
                return ServiceResult<bool>.Failure("Article update failed.");
            }
            else
            {
                return ServiceResult<bool>.Success(true);
            }
        }
        
        public async Task<ServiceResult<IEnumerable<ArticleDto>>> GetArticlesAsync(Func<Article, ArticleDto> mapper, CancellationToken token = default)
        {
            var result = await _articlerepository.GetArticlesAsync(token);
            var mappedResult = result.Select(x => mapper(x));
            
            return ServiceResult<IEnumerable<ArticleDto>>.Success(mappedResult);
        }
        public async Task<ServiceResult<ArticleDto>> GetArticleAsync(int id, Func<Article, ArticleDto> mapper, CancellationToken token = default)
        {
            var response = new ResponseTuple<ArticleDto, ResponseEnum>();

            var result = await _articlerepository.GetArticleAsync(id, token);
            if (result == null)
            {
                return ServiceResult<ArticleDto>.Failure("Article not found");
            }

            var articleDto = mapper(result);
            return ServiceResult<ArticleDto>.Success(articleDto);

        }

        public async Task<ServiceResult<bool>> CheckArticleAsync(int id, CancellationToken token = default)
        {
            var result = await _articlerepository.GetArticleAsync(id, token);
            if (result == null)
            {
                return ServiceResult<bool>.Failure("Article not found");
            }
            
            return ServiceResult<bool>.Success(true);
        }
        
        public async Task<ServiceResult<bool>> DeleteArticleAsync(int id, CancellationToken token = default)
        {

            var result = await _articlerepository.DeleteArticleAsync(id, token);
            if (result)
            {
                return ServiceResult<bool>.Success(true);
            }
            return ServiceResult<bool>.Failure("Article deletion error");

        }


    }
}

