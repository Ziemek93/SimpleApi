using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersInteractions.Domain.Contracts;

namespace UserInteractions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{

    private readonly ISender _sender;

    public CommentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("post/{Id}/[action]")]
    [Authorize(Policy = "IsUser")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Comments(int id, CancellationToken ct = default)
    {
        var request = new GetCommentsQuery
        {
            ArticleId = id
        };
        var response = await _sender.Send(request, ct);

        if (!response.Any())
        {
            return NotFound();
        }
        return Ok(response);
    }
    
    // [HttpPost("article/[action]")]
    // public async Task<IActionResult> Comment(AddCommentCommand request, CancellationToken ct = default)
    // {
    //     var response = await _sender.Send(request, ct);
    //
    //     if (!response.Any())
    //     {
    //         NotFound();
    //     }
    //     return Ok(response);
    // }
    //
}