using System.Security.Claims;
using FastEndpoints;
using MainApi.Models;
using MainApi.Models.Comments;
using MainApi.Services.ArticleService;
using MainApi.Services.UserService;
using MassTransit;

namespace MainApi.Endpoints.Private.SendMessage;

public class Endpoint : Endpoint<CreateChatMessageRequest, ResponseObject<bool>>
{
    private readonly IUserService _service;
    private readonly IPublishEndpoint _publishEndpoint;


    public Endpoint(IPublishEndpoint publishEndpoint, IUserService service)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }

    public override void Configure()
    {
        Post("/api/message/send");
        Claims(ClaimTypes.Name);
        Roles("User");
        // Policies("AdminOnly");
        Policies("IsUser");
    }

    public override async Task HandleAsync(CreateChatMessageRequest req, CancellationToken token = default)
    {
        var response = await _service.CheckChatUsersAsync(req.SenderId, req.RecipientId, token);

        if (!response.resultResponse)
        {
            await SendAsync(null, StatusCodes.Status404NotFound, token);
            return;
        }

        await _publishEndpoint.Publish<CreateChatMessageRequest>(req, token);

        await SendAsync(null, StatusCodes.Status204NoContent, token);
    }
}