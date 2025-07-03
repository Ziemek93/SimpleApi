using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersInteractions.Domain.Contracts;

namespace UserInteractions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ISender _sender;

    public ChatController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("[action]/{firstChatParticipantId}/{secondChatParticipantId}")]
    // [Authorize(Policy = "IsUser")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> LastMessages(int firstChatParticipantId, int secondChatParticipantId,
        CancellationToken ct = default)
    {
        var request = new GetLastChatMessagesQuery
        {
            FirstChatParticipantId = firstChatParticipantId,
            SecondChatParticipantId = secondChatParticipantId
        };

        var response = await _sender.Send(request, ct);

        if (!response.Any())
        {
            NotFound();
        }

        return Ok(response);
    }

    [HttpGet("[action]/{firstChatParticipantId}/{secondChatParticipantId}")]
    // [Authorize(Policy = "IsUser")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Messages(int firstChatParticipantId, int secondChatParticipantId,
        int? paginationSize, int? pageNumber, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var request = new GetChatMessagesQuery
        {
            FirstChatParticipantId = firstChatParticipantId,
            SecondChatParticipantId = secondChatParticipantId,
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