using System.Security.Claims;
using FastEndpoints;

namespace MainApi.Models.Comments;

public class CreateCommentRequest
{
    [BindFrom("id")]
    public int Id { get; set; }
    public string Content { get; set; }
    [FromClaim(ClaimTypes.Name)]
    public string Login { get; set; }
}