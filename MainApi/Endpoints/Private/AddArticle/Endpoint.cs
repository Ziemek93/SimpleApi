using FastEndpoints;
using System.Security.Claims;
using MainApi.Models.ArticlesDao;
using MainApi.Models.Entities;
using MainApi.Services.ArticleService;

namespace MainApi.Endpoints.Private.AddArticle;

public class Endpoint : Endpoint<AddArticleDao, Article, ArticleMapper>
{
    private readonly IArticleService _service;


    public Endpoint(IArticleService service)
    {
        _service = service;
    }
    public override void Configure()
    {
        Post("/api/article/create");
        Claims(ClaimTypes.Name);
        Roles("User");
        // Policies("AdminOnly");
        Policies("IsUser");
    }

    public override async Task HandleAsync(AddArticleDao req, CancellationToken token = default)
    {
        var article = Map.ToEntity(req);
        var result = await _service.AddArticleAsync(article, token);

        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }
        await SendOkAsync(result.Data, token);
    }

}