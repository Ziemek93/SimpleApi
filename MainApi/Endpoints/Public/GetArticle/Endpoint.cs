using FastEndpoints;
using MainApi.Models.ArticlesDto;
using MainApi.Services;
using MainApi.Services.ArticleService;

namespace MainApi.Endpoints.Public.GetArticle;

public class Endpoint : EndpointWithoutRequest<ArticleDto, ArticleMapper>
{
    private readonly IArticleService _service;


    public Endpoint(IArticleService service)
    {
        _service = service;
    }
    public override void Configure()
    {
        Get("/api/article/{id}");
        AllowAnonymous();

    }
    public override async Task HandleAsync(CancellationToken token = default)
    {
        int articleID = Route<int>("Id");
        var result = await _service.GetArticleAsync(articleID, Map.FromEntity);
        
        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }
        
        await SendOkAsync(result.Data, token);
        
    }

}
