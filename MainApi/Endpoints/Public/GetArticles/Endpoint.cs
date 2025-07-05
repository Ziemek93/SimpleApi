using FastEndpoints;
using MainApi.Models.ArticlesDto;
using MainApi.Services.ArticleService;

namespace MainApi.Endpoints.Public.GetArticles;

public class Endpoint : EndpointWithoutRequest<IEnumerable<ArticleDto>, ArticleMapper>
{
    private readonly IArticleService _service;


    public Endpoint(IArticleService service)
    {
        _service = service;
    }
    public override void Configure()
    {
        Get("/api/articles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken token = default)
    {
        var result = await _service.GetArticlesAsync(Map.FromEntity, token);

        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }
        await SendOkAsync(result.Data, token);
    }
}
