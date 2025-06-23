using FastEndpoints;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;

namespace MainApi.Endpoints.Public.GetArticle;

public class ArticleMapper : Mapper<int, ArticleDto, Article>, IResponseMapper
{
    public override ArticleDto FromEntity(Article a) => new()
    {
        ArticleName = a.Name,
        ArticleDescription = a.Description,
        Visibility = a.Visibility,
        Tags = a.Tags.Select(c => c.Title).ToList(),
    };
}