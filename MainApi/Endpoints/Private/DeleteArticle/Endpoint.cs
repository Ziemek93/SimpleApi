using FastEndpoints;
using System.Security.Claims;
using MainApi.Models;
using MainApi.Services.ArticleService;

namespace MainApi.Endpoints.Private.DeleteArticle;

public class Endpoint : Endpoint<RequestRoute, bool>
{
    private readonly IArticleService _service;


    public Endpoint(IArticleService service)
    {
        _service = service;
    }
    public override void Configure()
    {
        Delete("/api/article/{id}");
        Claims(ClaimTypes.Name);
        Roles("User");
    }

    public override async Task HandleAsync(RequestRoute route, CancellationToken token = default)
    {

        var result = await _service.DeleteArticleAsync(route.Id);
        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }
        
        await SendAsync(result.Data, StatusCodes.Status204NoContent, token);
    }

}

public class RequestRoute
{
    [BindFrom("id")]
    public int Id { get; set; }
}