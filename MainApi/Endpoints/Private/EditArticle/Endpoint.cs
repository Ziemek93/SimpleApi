using FastEndpoints;
using System.Security.Claims;
using MainApi.Models;
using MainApi.Models.ArticlesDao;
using MainApi.Services.ArticleService;

namespace MainApi.Endpoints.Private.EditArticle;

internal sealed class Endpoint : Endpoint<ArticleDao, bool>
{
    private readonly IArticleService _service;
    public Endpoint(IArticleService service)
    {
        _service = service;
    }
    public override void Configure()
    {
        Put("/api/article/{id}");
        Claims(ClaimTypes.Name);
        Roles("User");
        //Roles("Admin", "Manager");
        //Claims("Username", "EmployeeID");
        // AccessControl("Article_Create");
        // Permissions(Allow.Article_Create);
    }

    public override async Task HandleAsync(ArticleDao request, CancellationToken token = default)
    {
        int articleID = Route<int>("Id");
        
        var result = await _service.EditArticleAsync(articleID, request, token);
        
        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }

        await SendOkAsync(result.Data, token);
    }
}