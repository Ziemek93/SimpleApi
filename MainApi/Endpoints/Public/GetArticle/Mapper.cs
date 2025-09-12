using FastEndpoints;
using MainApi.Models.ArticlesDto;
using MainApi.Models.CategoriesDto;
using MainApi.Models.Entities;

namespace MainApi.Endpoints.Public.GetArticle;

public class ArticleMapper : Mapper<int, ArticleDto, Article>, IResponseMapper
{
    public override ArticleDto FromEntity(Article a) => new()
    {
        Id = a.ArticleId,
        Name = a.Name,
        Description = a.Description,
        Visibility = a.Visibility,
        Category = new CategoryDto
        {
            Id = a.CategoryId,
            Name = a.Category.CategoryName,
            Description = a.Category.CategoryDescription,
        },
        Tags = a.Tags.Select(c => c.Title).ToList(),
        // Comments = ...
    };
}