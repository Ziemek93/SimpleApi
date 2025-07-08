using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersInteractions.Application.Abstractions;
using UsersInteractions.Domain.Contracts;

namespace UserInteractions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public ChatController(ISender sender, IUserContext userContext)
    {
        _sender = sender;
        _userContext = userContext;
    }

    [HttpGet("[action]/{secondUserId}")]
    // [Authorize(Policy = "IsUser")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> LastMessages(int secondUserId,
        CancellationToken ct = default)
    {
        var request = new GetLastChatMessagesQuery
        {
            FirstChatParticipantId = _userContext.UserId,
            SecondChatParticipantId = secondUserId
        };

        var response = await _sender.Send(request, ct);

        if (!response.Any())
        {
            NotFound();
        }

        return Ok(response);
    }

    [HttpGet("[action]/{secondUserId}")]
    // [Authorize(Policy = "IsUser")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Messages(int secondUserId,
        int? paginationSize, int? pageNumber, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        
        
        var request = new GetChatMessagesQuery
        {
            FirstChatParticipantId = _userContext.UserId,
            SecondChatParticipantId = secondUserId,
            PaginationSize = paginationSize,
            PageNumber = pageNumber,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var response = await _sender.Send(request, ct);

        if (!response.Any())
        {
            return NotFound();
        }

        return Ok(response);
    }
}