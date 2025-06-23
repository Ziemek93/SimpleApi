using FastEndpoints;
using MainApi.Models.ArticlesDao;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;

namespace MainApi.Endpoints.Private.AddArticle;

public class ArticleMapper : Mapper<AddArticleDao, ArticleDtoExpanded, Article>, IResponseMapper
{
    public override Article ToEntity(AddArticleDao am) => new() // from user - step 1
    {
        Name = am.ArticleName,
        CategoryId = am.CategoryId,
        Description = am.ArticleDescription,
        Visibility = am.Visibility,
        Tags = am.Tags
    };

    //public override ArticleDtoExpanded FromEntity(Article a) => new() // to db - setp 2
    //{
    //    ArticleId = a.ArticleId,
    //    Name = a.Name,
    //    Description = a.Description,
    //    Category = a.Category,
    //    Visibility = a.Visibility,
    //    Comments = a.Comments,
    //    Tags = a.Tags
    //};

}