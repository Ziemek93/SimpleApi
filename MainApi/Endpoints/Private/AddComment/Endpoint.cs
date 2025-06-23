using System.Security.Claims;
using FastEndpoints;
using MainApi.Models;
using MainApi.Models.ArticlesDao;
using MainApi.Models.Comments;
using MainApi.Models.Entities;
using MainApi.Services.ArticleService;
using MassTransit;

namespace MainApi.Endpoints.Private.AddComment;

public class Endpoint : Endpoint<CreateCommentRequest, bool>
{
    private readonly IArticleService _service;
    private readonly IPublishEndpoint _publishEndpoint;


    public Endpoint(IPublishEndpoint publishEndpoint, IArticleService service)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }
    public override void Configure()
    {
        Post("/api/article/{id}/comment");
        Claims(ClaimTypes.Name);
        Roles("User");
        // Policies("AdminOnly");
        Policies("IsUser");
    }

    public override async Task HandleAsync(CreateCommentRequest req, CancellationToken token = default)
    {
        var result = await _service.CheckArticleAsync(req.Id, token);

        if (result.IsFailure)
        {
            AddError(result.Errors.First());
            await SendNotFoundAsync(token);
            return;
        }

        await _publishEndpoint.Publish<CreateCommentRequest>(req, token);
        await SendAsync(result.Data, StatusCodes.Status204NoContent, token);
    }

}