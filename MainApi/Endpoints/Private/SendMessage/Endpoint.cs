using System.Security.Claims;
using FastEndpoints;
using MainApi.Models;
using MainApi.Models.Comments;
using MainApi.Services.UserService;
using MassTransit;
using Shared.Broker.Contracts;

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
        // Policies("IsUser");
    }

    public override async Task HandleAsync(CreateChatMessageRequest req, CancellationToken token = default)
    {
        if (!int.TryParse(req.UserId, out var senderId))
        {
            AddError("User id have incorrect format.");
            await SendErrorsAsync(400, token);
            return;
        }
        
        var response = await _service.CheckChatUsersAsync(senderId, req.RecipientId, token);

        if (!response.resultResponse)
        {
            await SendNotFoundAsync(token);
            return;
        }

        var contract = new CreateChatMessageContract
        {
            SenderId = senderId,
            RecipientId = req.RecipientId,
            Content = req.Content,
        };
        
        await _publishEndpoint.Publish<CreateChatMessageContract>(contract, token);

        await SendNoContentAsync(token);
    }
}